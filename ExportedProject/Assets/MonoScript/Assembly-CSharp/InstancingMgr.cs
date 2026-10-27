using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InstancingMgr
{
	public static InstancingMgr Ins = new InstancingMgr();

	public bool IsSupport;

	private readonly InstancingObjPool mObjPool = new InstancingObjPool();

	private readonly InstancingDrawCallPool mDrawCallPool = new InstancingDrawCallPool();

	private readonly Dictionary<int, Mesh> mMeshs = new Dictionary<int, Mesh>();

	private readonly Dictionary<int, Material> mMaterials = new Dictionary<int, Material>();

	private readonly Dictionary<Renderer, InstancingObj> mInstancingRenderers = new Dictionary<Renderer, InstancingObj>();

	private long mNextId = 1L;

	private readonly Dictionary<long, InstancingObj> mInstancingObjs = new Dictionary<long, InstancingObj>();

	private readonly List<InstancingObj> mReadyInstancingObjs = new List<InstancingObj>(1024);

	private bool mNeedRePreprareDrawCall = true;

	private bool mDrawcallPreparing;

	private List<InstancingDrawCall> mDrawCalls = new List<InstancingDrawCall>(300);

	private List<InstancingDrawCall> mPreparedDrawCalls = new List<InstancingDrawCall>(300);

	private Camera mCamera;

	private InstancingMgr()
	{
	}

	public void Init(Camera camera)
	{
		IsSupport = SystemInfo.supportsInstancing && SystemInfo.graphicsShaderLevel >= 45;
		mCamera = camera;
		mDrawcallPreparing = false;
		mNeedRePreprareDrawCall = true;
	}

	public void Clear()
	{
		mMeshs.Clear();
		mMaterials.Clear();
		mInstancingRenderers.Clear();
		int i = 1;
		for (int count = mReadyInstancingObjs.Count; i < count; i++)
		{
			mObjPool.Recycle(mReadyInstancingObjs[i]);
		}
		mReadyInstancingObjs.Clear();
		int j = 1;
		for (int count2 = mDrawCalls.Count; j < count2; j++)
		{
			mDrawCallPool.Recycle(mDrawCalls[j]);
		}
		mDrawCalls.Clear();
		int k = 1;
		for (int count3 = mPreparedDrawCalls.Count; k < count3; k++)
		{
			mDrawCallPool.Recycle(mPreparedDrawCalls[k]);
		}
		mPreparedDrawCalls.Clear();
	}

	public bool RegisRenderer(Renderer renderer, Mesh mesh)
	{
		InstancingObj value;
		if (mInstancingRenderers.TryGetValue(renderer, out value))
		{
			return false;
		}
		int instanceID = mesh.GetInstanceID();
		int instanceID2 = renderer.sharedMaterial.GetInstanceID();
		Mesh value2;
		if (!mMeshs.TryGetValue(instanceID, out value2) || !value2)
		{
			mMeshs[instanceID] = mesh;
		}
		Material value3;
		if (!mMaterials.TryGetValue(instanceID2, out value3) || !value3)
		{
			mMaterials[instanceID2] = renderer.sharedMaterial;
		}
		value = mObjPool.Get();
		value.meshInsId = instanceID;
		value.materialInsId = instanceID2;
		value.matrix = renderer.localToWorldMatrix;
		mInstancingRenderers.Add(renderer, value);
		mNeedRePreprareDrawCall = true;
		return true;
	}

	public void UnRegisRenderer(Renderer renderer)
	{
		InstancingObj value;
		if (mInstancingRenderers.TryGetValue(renderer, out value))
		{
			mInstancingRenderers.Remove(renderer);
			mObjPool.Recycle(value);
			mNeedRePreprareDrawCall = true;
		}
	}

	public long AddInstancingObj(Mesh mesh, Material material, Matrix4x4 matrix, bool castShadows, bool receiveShadows)
	{
		mNeedRePreprareDrawCall = true;
		int instanceID = mesh.GetInstanceID();
		int instanceID2 = material.GetInstanceID();
		Mesh value;
		if (!mMeshs.TryGetValue(instanceID, out value) || !value)
		{
			mMeshs[instanceID] = mesh;
		}
		Material value2;
		if (!mMaterials.TryGetValue(instanceID2, out value2) || !value2)
		{
			mMaterials[instanceID2] = material;
		}
		InstancingObj instancingObj = mObjPool.Get();
		instancingObj.id = mNextId++;
		instancingObj.meshInsId = instanceID;
		instancingObj.materialInsId = instanceID2;
		instancingObj.matrix = matrix;
		instancingObj.castShadows = castShadows;
		instancingObj.receiveShadows = receiveShadows;
		mInstancingObjs.Add(instancingObj.id, instancingObj);
		return instancingObj.id;
	}

	public void removeInstancingObj(long id)
	{
		InstancingObj value;
		if (mInstancingObjs.TryGetValue(id, out value))
		{
			mNeedRePreprareDrawCall = true;
			mInstancingObjs.Remove(id);
			mObjPool.Recycle(value);
		}
	}

	public void Update()
	{
		if (!IsSupport)
		{
			return;
		}
		Draw();
		if (mNeedRePreprareDrawCall && !mDrawcallPreparing)
		{
			mNeedRePreprareDrawCall = false;
			UpdateReadyObjs();
			if (mReadyInstancingObjs.Count > 0)
			{
				mDrawcallPreparing = true;
				Worker.Ins.Insert(PrepareDrawCalls, true);
			}
		}
	}

	public void Draw()
	{
		int i = 0;
		for (int count = mDrawCalls.Count; i < count; i++)
		{
			InstancingDrawCall instancingDrawCall = mDrawCalls[i];
			Mesh value;
			Material value2;
			if (mMeshs.TryGetValue(instancingDrawCall.meshInsId, out value) && (bool)value && mMaterials.TryGetValue(instancingDrawCall.materialInsId, out value2) && (bool)value2)
			{
				Graphics.DrawMeshInstanced(value, 0, value2, instancingDrawCall.matrices, instancingDrawCall.objCount, null, instancingDrawCall.castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, instancingDrawCall.receiveShadows, 0, mCamera);
			}
		}
	}

	private void UpdateReadyObjs()
	{
		mReadyInstancingObjs.Clear();
		if (mInstancingRenderers.Count > 0)
		{
			foreach (Renderer key in mInstancingRenderers.Keys)
			{
				if (key.enabled)
				{
					mReadyInstancingObjs.Add(mInstancingRenderers[key]);
				}
			}
		}
		if (mInstancingObjs.Count > 0)
		{
			mReadyInstancingObjs.AddRange(mInstancingObjs.Values);
		}
	}

	private void PrepareDrawCalls()
	{
		mReadyInstancingObjs.Sort();
		int i = 0;
		for (int count = mPreparedDrawCalls.Count; i < count; i++)
		{
			mDrawCallPool.Recycle(mPreparedDrawCalls[i]);
		}
		mPreparedDrawCalls.Clear();
		InstancingDrawCall instancingDrawCall = mDrawCallPool.Get();
		InstancingObj instancingObj = mReadyInstancingObjs[0];
		instancingDrawCall.meshInsId = instancingObj.meshInsId;
		instancingDrawCall.materialInsId = instancingObj.materialInsId;
		instancingDrawCall.matrices[0] = instancingObj.matrix;
		instancingDrawCall.receiveShadows = instancingObj.receiveShadows;
		instancingDrawCall.castShadows = instancingObj.castShadows;
		instancingDrawCall.objCount = 1;
		int j = 1;
		for (int count2 = mReadyInstancingObjs.Count; j < count2; j++)
		{
			InstancingObj instancingObj2 = mReadyInstancingObjs[j];
			if (instancingObj2.meshInsId == instancingDrawCall.meshInsId && instancingObj2.materialInsId == instancingDrawCall.materialInsId && instancingDrawCall.objCount < 1000)
			{
				instancingDrawCall.matrices[instancingDrawCall.objCount++] = instancingObj2.matrix;
				continue;
			}
			mPreparedDrawCalls.Add(instancingDrawCall);
			instancingDrawCall = mDrawCallPool.Get();
			instancingDrawCall.meshInsId = instancingObj2.meshInsId;
			instancingDrawCall.materialInsId = instancingObj2.materialInsId;
			instancingDrawCall.matrices[0] = instancingObj2.matrix;
			instancingDrawCall.receiveShadows = instancingObj.receiveShadows;
			instancingDrawCall.castShadows = instancingObj.castShadows;
			instancingDrawCall.objCount = 1;
		}
		mPreparedDrawCalls.Add(instancingDrawCall);
		List<InstancingDrawCall> list = mDrawCalls;
		mDrawCalls = mPreparedDrawCalls;
		mPreparedDrawCalls = list;
		mDrawcallPreparing = false;
	}
}
