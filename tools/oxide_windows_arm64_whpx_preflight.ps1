#requires -Version 7.0
<#
Read-only Windows 11 ARM64 WHPX capability preflight.
Queries WHvGetCapability only: does not create VM partitions, download Android,
load APKs, alter Play Integrity, request elevation or modify host settings.
#>
param(
    [string]$Output = "reports/oxide-windows-arm64-whpx-preflight.json",
    [switch]$SelfTest
)
$ErrorActionPreference = "Stop"

function Get-WhpxClassification {
    param(
        [string]$Architecture,
        [int]$WindowsBuild,
        [int]$HypervisorHresult,
        [ulong]$HypervisorValue,
        [int]$FeaturesHresult,
        [ulong]$FeaturesValue
    )
    if ($Architecture -ne "Arm64") { return "NOT_NATIVE_WINDOWS_ARM64" }
    if ($WindowsBuild -lt 26100) { return "WINDOWS_ARM64_BUILD_TOO_OLD" }
    if ($HypervisorHresult -ne 0) { return "WHPX_HYPERVISOR_QUERY_FAILED" }
    if (($HypervisorValue -band [ulong]1) -eq 0) {
        return "WHPX_HYPERVISOR_NOT_PRESENT"
    }
    if ($FeaturesHresult -ne 0) { return "WHPX_FEATURES_QUERY_FAILED" }
    # Windows SDK WHV_CAPABILITY_FEATURES: Arm64Support is bit 11.
    if (($FeaturesValue -band [ulong]2048) -eq 0) {
        return "WHPX_ARM64_FEATURE_NOT_EXPOSED"
    }
    return "WHPX_ARM64_PLATFORM_PREREQUISITE_MET"
}

if ($SelfTest) {
    $cases = @(
        @{name="ready"; arch="Arm64"; build=26200; hyper=0; hv=[ulong]1; features=0; bits=[ulong]2048; expected="WHPX_ARM64_PLATFORM_PREREQUISITE_MET"},
        @{name="x64"; arch="X64"; build=26200; hyper=0; hv=[ulong]1; features=0; bits=[ulong]2048; expected="NOT_NATIVE_WINDOWS_ARM64"},
        @{name="older_os"; arch="Arm64"; build=22000; hyper=0; hv=[ulong]1; features=0; bits=[ulong]2048; expected="WINDOWS_ARM64_BUILD_TOO_OLD"},
        @{name="no_hypervisor"; arch="Arm64"; build=26200; hyper=0; hv=[ulong]0; features=0; bits=[ulong]2048; expected="WHPX_HYPERVISOR_NOT_PRESENT"},
        @{name="query_denied"; arch="Arm64"; build=26200; hyper=-1; hv=[ulong]1; features=0; bits=[ulong]2048; expected="WHPX_HYPERVISOR_QUERY_FAILED"},
        @{name="features_denied"; arch="Arm64"; build=26200; hyper=0; hv=[ulong]1; features=-1; bits=[ulong]2048; expected="WHPX_FEATURES_QUERY_FAILED"},
        @{name="arm64_missing"; arch="Arm64"; build=26200; hyper=0; hv=[ulong]1; features=0; bits=[ulong]1024; expected="WHPX_ARM64_FEATURE_NOT_EXPOSED"},
        @{name="arm64_bit_with_other_bits"; arch="Arm64"; build=26200; hyper=0; hv=[ulong]1; features=0; bits=[ulong]3072; expected="WHPX_ARM64_PLATFORM_PREREQUISITE_MET"}
    )
    foreach ($case in $cases) {
        $parameters = @{
            Architecture = $case.arch
            WindowsBuild = $case.build
            HypervisorHresult = $case.hyper
            HypervisorValue = $case.hv
            FeaturesHresult = $case.features
            FeaturesValue = $case.bits
        }
        $actual = Get-WhpxClassification @parameters
        if ($actual -ne $case.expected) {
            throw "Synthetic preflight fixture '$($case.name)': $actual != $($case.expected)"
        }
    }
    Write-Output "Ran $($cases.Count) synthetic WHPX classification tests: OK"
    exit 0
}

$architecture = [System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture.ToString()
$build = [System.Environment]::OSVersion.Version.Build
[ulong]$hyper = 0
[ulong]$features = 0
[uint32]$writtenHyper = 0
[uint32]$writtenFeatures = 0
[int]$hyperHr = -1
[int]$featuresHr = -1
$nativeError = $null

if ($architecture -eq "Arm64" -and $build -ge 26100) {
    try {
        Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class OxideWindowsWhpxQuery {
    [DllImport("WinHvPlatform.dll", EntryPoint="WHvGetCapability", ExactSpelling=true)]
    public static extern int GetCapability(
        uint code, out ulong value, uint bufferBytes, out uint writtenBytes);
}
'@ -ErrorAction Stop
        # WHvCapabilityCodeHypervisorPresent == 0
        $hyperHr = [OxideWindowsWhpxQuery]::GetCapability(
            [uint32]0, [ref]$hyper, [uint32]8, [ref]$writtenHyper)
        if ($hyperHr -eq 0 -and $writtenHyper -ge 1 -and
            (($hyper -band [ulong]1) -ne 0)) {
            # WHvCapabilityCodeFeatures == 1
            $featuresHr = [OxideWindowsWhpxQuery]::GetCapability(
                [uint32]1, [ref]$features, [uint32]8, [ref]$writtenFeatures)
            if ($featuresHr -eq 0 -and $writtenFeatures -lt 8) {
                $featuresHr = -1
            }
        }
        if ($hyperHr -eq 0 -and $writtenHyper -lt 1) {
            $hyperHr = -1
        }
    }
    catch {
        # No elevation, OS features, SDK or hypervisor settings are changed.
        $nativeError = $_.Exception.GetType().Name
    }
}

$parameters = @{
    Architecture = $architecture
    WindowsBuild = $build
    HypervisorHresult = $hyperHr
    HypervisorValue = $hyper
    FeaturesHresult = $featuresHr
    FeaturesValue = $features
}
$verdict = Get-WhpxClassification @parameters
$ready = $verdict -eq "WHPX_ARM64_PLATFORM_PREREQUISITE_MET"
$report = [ordered]@{
    status = $verdict
    host_architecture = $architecture
    windows_build = $build
    runner_image = $env:ImageOS
    windows_hypervisor_capability_hresult = $hyperHr
    windows_hypervisor_present = (($hyperHr -eq 0) -and (($hyper -band [ulong]1) -ne 0))
    windows_arm64_capability_hresult = $featuresHr
    whpx_arm64_support_bit = (($featuresHr -eq 0) -and
                              (($features -band [ulong]2048) -ne 0))
    whpx_arm64_prerequisite_verified = $ready
    probe_exception_type = $nativeError
    official_android_emulator_available = $false
    official_arm64_gms_system_image_available = $false
    original_android_vm_started = $false
    original_oxide_apks_downloaded = $false
    original_oxide_apks_modified = $false
    original_gameplay_verified = $false
    activity_original_35s_verified = $false
    heavy_android_retry_authorized = $false
    security_trigger = "UNKNOWN"
    next_action = if ($ready) {
        "CHECK_OFFICIAL_EMULATOR_ARM64_WINDOWS_HOST_SUPPORT_WITHOUT_DOWNLOAD"
    } else {
        "STOP_NO_ARM64_WHPX_CAPABILITY_ON_THIS_RUNNER"
    }
    limitations = @(
        "WHPX Arm64Support queries describe Windows hypervisor API, not Android emulator support.",
        "Hypervisor exposure does not establish that nested virtualization can start an ARM64 Android guest.",
        "Even a usable emulator does not certify device integrity or guarantee game acceptance.",
        "No VM partition is created. No original APK or Android image is downloaded."
    )
}
$directory = Split-Path -Parent $Output
if ($directory) { New-Item -ItemType Directory -Force -Path $directory | Out-Null }
$report | ConvertTo-Json -Depth 6 | Set-Content -Path $Output -Encoding utf8
$report | ConvertTo-Json -Depth 6 | Write-Output
# A negative WHPX verdict is a valid diagnostic, not a workflow failure.
exit 0
