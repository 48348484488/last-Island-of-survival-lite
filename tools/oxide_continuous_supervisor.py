#!/usr/bin/env python3
"""Event-based CI handoff and watchdog. No AI agent or APK modification."""
import datetime as dt
import json
import os
import sys
import urllib.request

REPO = "48348484488/oxide"
SOURCES = {
    "oxide-arm64-signal-observability.yml": 1,
    "oxide-kernel-signal-replay.yml": 1,
    "oxide-original-launch-milestones.yml": 1,
    "oxide-native-google-play-discovery.yml": 3,
    "redroid-original-smoke.yml": 4,
    "original-overlay-native-arm64.yml": 5,
    "oxide-virtual-overlay-states.yml": 5,
    "oxide-m3-archived-android-matrix.yml": 3,
    "oxide-m3-android15-16-boot.yml": 3,
    "oxide-m3-android16-boot-diagnostics.yml": 3,
    "oxide-m3-android15-original-smoke.yml": 4,
    "oxide-virtual-overlay-android15.yml": 5,
    "oxide-m5-android16-original-overlay.yml": 5,
    "oxide-m3-android16-ashmem-prereqs.yml": 3,
    "oxide-m3-ashmem617-build-only.yml": 3,
    "oxide-m3-android16-ashmem-real-boot.yml": 3,
    "oxide-m3-android16-original-ashmem.yml": 4,
    "oxide-original-exit-order-3-versions.yml": 1,
    "oxide-m4-android16-binder-finish-origin.yml": 4,
    "oxide-m4-binder-finish-control-replay.yml": 4,
    "oxide-m4-task-removal-audit.yml": 4,
    "oxide-m4-binder-host-capabilities.yml": 4,

}
NEXT = {3: "oxide-native-google-play-discovery.yml",
        4: "redroid-original-smoke.yml",
        5: "original-overlay-native-arm64.yml",
        6: "oxide-virtual-overlay-states.yml"}

def target(run):
    if run.get("head_branch") != "main" or run.get("head_repository", {}).get("full_name") != REPO:
        return None
    return SOURCES.get(str(run.get("path", "")).rsplit("/", 1)[-1])

def old_run(run, now):
    issue = target(run)
    status = run.get("status")
    if issue is None or status not in ("queued", "in_progress"):
        return None
    expected_runtime = {"redroid-original-smoke.yml": 27,
                        "oxide-kernel-signal-replay.yml": 10,
                        "oxide-original-launch-milestones.yml": 10,
                        "oxide-arm64-signal-observability.yml": 12,
                        "oxide-native-google-play-discovery.yml": 11,
                        "original-overlay-native-arm64.yml": 40,
                        "oxide-virtual-overlay-states.yml": 30}
    wf = str(run.get("path", "")).rsplit("/", 1)[-1]
    threshold = expected_runtime.get(wf, 25) if status == "in_progress" else 25
    field = "run_started_at" if status == "in_progress" else "created_at"
    when = run.get(field)
    if not when:
        return None
    start = dt.datetime.fromisoformat(when.replace("Z", "+00:00"))
    age = (now - start).total_seconds() / 60
    return (issue, age, threshold) if age >= threshold else None

def plan_next(closed, states):
    if closed not in range(1, 6):
        return None
    pending = next((n for n in range(1, 7) if states.get(n) == "open"), None)
    if pending is None or pending <= closed:
        return None
    if any(states.get(n) != "closed" for n in range(1, pending)):
        return None
    return (pending, NEXT[pending]) if pending in NEXT else None

# Only chain low-cost, read-only diagnostic workflows; never auto-launch the
# expensive original APK (~867 MB) or a potential security-sensitive experiment.
ON_SUCCESS = {
    "oxide-arm64-signal-observability.yml": "oxide-kernel-signal-replay.yml",
}

def lightweight_handoff(source, issue_state):
    """Choose a fixed, acyclic follow-up; no AI coding or milestone closure."""
    if issue_state != "open":
        return None
    return ON_SUCCESS.get(source)

def maybe_handoff(api, run):
    workflow = lightweight_handoff(str(run.get("path", "")).rsplit("/", 1)[-1],
                                  api.api("/issues/1").get("state"))
    if workflow is None:
        print("NO_AUTOMATIC_LIGHTWEIGHT_FOLLOWUP")
        return
    source_id = int(run["id"])
    attempt = int(run.get("run_attempt") or 1)
    marker = "oxide-auto:followup:%d:%d:%s" % (source_id, attempt, workflow)
    comments = api.api("/issues/1/comments?per_page=100")
    if any(marker in c.get("body", "") for c in comments):
        print("FOLLOWUP_ALREADY_DISPATCHED", marker)
        return
    # workflow_dispatch can start a new run even when called from GITHUB_TOKEN.
    # The follow-up workflow must still pass its own tests and cannot close an issue.
    api.api("/actions/workflows/" + workflow + "/dispatches",
            "POST", {"ref": "main"})
    api.record(1, marker,
               "**Etapa leve iniciada automaticamente por evento**\n\n"
               "Execucao anterior: https://github.com/%s/actions/runs/%d\n"
               "Workflow seguinte: `%s`.\n"
               "Nenhum APK foi alterado ou executado nesta transicao; "
               "a meta continua aberta ate comprovacao separada." % (REPO, source_id, workflow))

class GitHub:
    def __init__(self, token):
        if not token:
            raise RuntimeError("Missing GH_TOKEN")
        self.token = token
    def api(self, endpoint, method="GET", payload=None):
        req = urllib.request.Request(
            "https://api.github.com/repos/" + REPO + endpoint,
            data=None if payload is None else json.dumps(payload).encode(),
            headers={"Authorization": "Bearer " + self.token,
                     "Accept": "application/vnd.github+json",
                     "X-GitHub-Api-Version": "2022-11-28"},
            method=method)
        with urllib.request.urlopen(req, timeout=25) as response:
            raw = response.read()
        return json.loads(raw) if raw else {}
    def record(self, number, marker, body):
        comments = self.api("/issues/%d/comments?per_page=100" % number)
        if any(marker in c.get("body", "") for c in comments):
            print("ALREADY_RECORDED", marker)
            return False
        self.api("/issues/%d/comments" % number, "POST", {"body": "<!-- " + marker + " -->\n" + body})
        print("COMMENT_CREATED", number, marker)
        return True

def complete(api, event):
    run = event.get("workflow_run", {})
    issue = target(run)
    if event.get("action") != "completed" or issue is None or run.get("status") != "completed":
        return
    run_id = int(run["id"])
    result = run.get("conclusion", "unknown")
    link = "https://github.com/" + REPO + "/actions/runs/" + str(run_id)
    if result == "success":
        recommendation = ("Etapa de CI aprovada; conferir criterios da meta e continuar "
                          "com a proxima tarefa nao concluida. CI aprovado nao prova gameplay.")
    else:
        recommendation = ("Etapa falhou/cancelou; analisar log e classificar a falha. "
                          "Nao reiniciar automaticamente sem saber a causa.")
    api.record(issue, "oxide-auto:run:%s:%s" % (run_id, run.get("run_attempt", 1)),
               "**Continuacao automatica do Oxide**\n\nExecucao: " + link +
               "\nResultado: " + result + ".\n\n" + recommendation)
    if result == "success" and issue == 1:
        maybe_handoff(api, run)

def closed_issue(api, event):
    if event.get("action") != "closed" or event.get("repository", {}).get("full_name") != REPO:
        return
    issue = event.get("issue", {})
    number = issue.get("number")
    if not isinstance(number, int) or issue.get("state") != "closed":
        return
    rows = api.api("/issues?state=all&per_page=50")
    states = {x["number"]: x["state"] for x in rows if "pull_request" not in x}
    plan = plan_next(number, states)
    if not plan:
        print("NO_SAFE_NEXT_MILESTONE")
        return
    next_number, workflow = plan
    marker = "oxide-auto:transition:%d:%d" % (number, next_number)
    if any(marker in c.get("body", "") for c in api.api("/issues/%d/comments?per_page=100" % next_number)):
        print("TRANSITION_ALREADY_PROCESSED")
        return
    api.api("/actions/workflows/" + workflow + "/dispatches", "POST", {"ref": "main"})
    api.record(next_number, marker,
               "**Continuidade por encerramento de meta**\n\nA meta #%d foi fechada. "
               "O teste existente %s foi iniciado na branch main. "
               "Isto nao conclui automaticamente a proxima meta." % (number, workflow))

def watchdog(api, now):
    for status in ("in_progress", "queued"):
        runs = api.api("/actions/runs?status=" + status + "&per_page=100").get("workflow_runs", [])
        for run in runs:
            if not isinstance(run.get("id"), int):
                continue
            alert = old_run(run, now)
            if alert:
                issue, age, limit = alert
                marker = "oxide-auto:stalled:%s:%s" % (run["id"], run.get("run_attempt", 1))
                api.record(issue, marker,
                           "**Possivel travamento no GitHub Actions**\n\nExecucao: "
                           "https://github.com/%s/actions/runs/%d\n" % (REPO, run["id"]) +
                           "Estado: %s; tempo: %.1f minutos (limite: %d).\n" %
                           (run["status"], age, limit) +
                           "Verificar logs/timeout. Sem cancelamento ou repeticao automatica.")

def main():
    event_name = os.environ.get("GITHUB_EVENT_NAME", "workflow_dispatch")
    event_path = os.environ.get("GITHUB_EVENT_PATH")
    event = json.load(open(event_path)) if event_path else {}
    api = GitHub(os.environ.get("GH_TOKEN"))
    if event_name == "workflow_run":
        complete(api, event)
    elif event_name == "issues":
        closed_issue(api, event)
    else:
        watchdog(api, dt.datetime.now(dt.timezone.utc))

if __name__ == "__main__":
    main()
