"""Source regression for allocation-free repeated skeletal pose updates.

This checks buffer lifetime, not FPS improvement or original animation parity.
Android build and measured runtime pacing remain authoritative.
"""
import unittest
from pathlib import Path

HEADER = (Path(__file__).resolve().parents[1] /
          "last_island_recreated_game" / "native_original_animation.hpp")
MAIN = HEADER.parent / "main.cpp"


class SkinningScratchTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.source = HEADER.read_text(encoding="utf-8")

    def test_vertex_and_normal_scratch_are_owned_by_mesh(self):
        for token in ("std::vector<float> poseVertices,poseNormals;",
                      "auto &deformed=part.poseVertices;",
                      "auto &normals=part.poseNormals;",
                      "deformed.assign(size_t(part.vertexCount)*3,0.f);",
                      "normals.assign(size_t(part.vertexCount)*3,0.f);"):
            self.assertIn(token, self.source)
        self.assertNotIn(
            "std::vector<float> deformed(size_t(part.vertexCount)*3,0);",
            self.source)

    def test_palette_and_bone_scratch_are_reused(self):
        for token in ("std::vector<Mat> poseMatrices;",
                      "auto &matrices=part.poseMatrices;",
                      "matrices.resize(part.palette.size());",
                      "std::vector<Mat> localScratch;",
                      "localScratch.resize(bc);",
                      "doneScratch.assign(bc,0);",
                      "busyScratch.assign(bc,0);"):
            self.assertIn(token, self.source)

    def test_inverse_bind_is_decoded_once_and_used_directly(self):
        self.assertIn("std::vector<Mat> inverseBind;", self.source)
        self.assertIn("m.inverseBind.resize(palettes);", self.source)
        self.assertIn("m.inverseBind[bi]=bind;", self.source)
        self.assertIn("multiply(world[part.palette[bi]],part.inverseBind[bi])", self.source)
        pose = self.source.split("void pose(", 1)[1].split("void setTextures(", 1)[0]
        self.assertNotIn("Mat rhs=ident();", pose)
        self.assertNotIn("multiply(multiply(LH", pose)

    def test_cpu_phase_telemetry_is_separated_from_gpu_submission(self):
        self.assertIn("lastPoseCpuMs=std::max(0.0,poseMs-vboMs)", self.source)
        self.assertIn("lastVboSubmitMs=vboMs;", self.source)
        self.assertIn("std::chrono::steady_clock::now()-vboStart", self.source)
        main = MAIN.read_text(encoding="utf-8")
        self.assertIn("LIOS_PHASE_TIMING frames=60", main)
        self.assertIn("phaseEndTotal+=endDrawingMs;", main)
        self.assertIn("phaseWorkTotal+=frameWorkMs;", main)

    def test_generic_edgeai_skinning_kernel_is_used(self):
        self.assertIn('#include "native_lbs_kernel.hpp"', self.source)
        self.assertIn("edgeai_lbs::skin_vertex(matrixData,", self.source)
        self.assertIn("const float* matrixData=matrices.front().data();", self.source)
        self.assertNotIn("for(int influence=0;influence<part.influences;influence++)", self.source)

    def test_original_gpu_skinning_uploads_are_preserved(self):
        self.assertIn("UpdateMeshBuffer(gpu,0,deformed.data()", self.source)
        self.assertIn("UpdateMeshBuffer(gpu,2,normals.data()", self.source)


if __name__ == "__main__":
    unittest.main()
