"""Source regression for allocation-free repeated skeletal pose updates.

This checks buffer lifetime, not FPS improvement or original animation parity.
Android build and measured runtime pacing remain authoritative.
"""
import unittest
from pathlib import Path

HEADER = (Path(__file__).resolve().parents[1] /
          "last_island_recreated_game" / "native_original_animation.hpp")


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
                      "matrices.clear();",
                      "std::vector<Mat> localScratch;",
                      "localScratch.resize(bc);",
                      "doneScratch.assign(bc,0);",
                      "busyScratch.assign(bc,0);"):
            self.assertIn(token, self.source)

    def test_original_gpu_skinning_uploads_are_preserved(self):
        self.assertIn("UpdateMeshBuffer(gpu,0,deformed.data()", self.source)
        self.assertIn("UpdateMeshBuffer(gpu,2,normals.data()", self.source)


if __name__ == "__main__":
    unittest.main()
