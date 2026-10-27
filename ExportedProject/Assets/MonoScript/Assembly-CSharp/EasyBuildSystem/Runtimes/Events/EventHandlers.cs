using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using Share;
using cfg;

namespace EasyBuildSystem.Runtimes.Events
{
	public class EventHandlers
	{
		public delegate void EventHandlerChangeBuildMode(BuildMode mode);

		public delegate void EventHandlerChangePartState(PartBehaviour part, StateType state);

		public delegate void EventHandlerPreviewCreated(PartBehaviour part);

		public delegate void EventHandlerPreviewCanceled(PartBehaviour part);

		public delegate void EventHandlerPlacedPart(PartBehaviour part, SocketBehaviour socket);

		public delegate void EventHandlerDestoryBuilding(long insId);

		public delegate void EventHandlerChangedAppearancePart(PartBehaviour part, int appearanceIndex);

		public delegate void EventHandlerDestroyedPart(PartBehaviour part);

		public delegate void EventHandlerEditedPart(PartBehaviour part, SocketBehaviour socket);

		public delegate void EventHandlerStorageSaving();

		public delegate void EventHandlerStorageLoading();

		public delegate void EventHandlerStorageLoadingDone(PartBehaviour[] Parts);

		public delegate void EventHandlerStorageSavingDone(PartBehaviour[] Parts);

		public delegate void EventHandlerStorageFailed(string exception);

		public delegate void EventHandlerStorageDeleted();

		public delegate void BuildPartDelegate(long InsId, BuildPart buildPartCfg, int status, Octets extraInfoOc);

		public delegate void BuildPartDestory(long InsId);

		public delegate void BuildPartUpdate(long InsId, BuildPart buildPartCfg);

		public static Utils.IntDelegate OnAddExtraBtn;

		public static Utils.IntDelegate OnRemoveExtraBtn;

		public static Utils.IntDelegate OnClickExtraBtn;

		public static Utils.LongDelegate OnAimedPart;

		public static Utils.LongDelegate OnAimedGo;

		public static BuildPartDelegate OnBuildPart;

		public static BuildPartDestory OnBuildPartDestory;

		public static BuildPartUpdate OnBuildPartUpdate;

		public static event EventHandlerChangeBuildMode OnBuildModeChanged;

		public static event EventHandlerChangePartState OnChangePartState;

		public static event EventHandlerPreviewCreated OnPreviewCreated;

		public static event EventHandlerPreviewCanceled OnPreviewCanceled;

		public static event EventHandlerPlacedPart OnPlacedPart;

		public static event EventHandlerPlacedPart OnWantPlacedPart;

		public static event EventHandlerDestoryBuilding OnWantDestoryBuilding;

		public static event EventHandlerChangedAppearancePart OnChangedAppearance;

		public static event EventHandlerDestroyedPart OnDestroyedPart;

		public static event EventHandlerEditedPart OnEditedPart;

		public static event EventHandlerStorageSaving OnStorageSaving;

		public static event EventHandlerStorageLoading OnStorageLoading;

		public static event EventHandlerStorageLoadingDone OnStorageLoadingDone;

		public static event EventHandlerStorageSavingDone OnStorageSavingDone;

		public static event EventHandlerStorageFailed OnStorageFailed;

		public static event EventHandlerStorageDeleted OnStorageDeleted;

		public static void BuildModeChanged(BuildMode mode)
		{
			if (EventHandlers.OnBuildModeChanged != null)
			{
				EventHandlers.OnBuildModeChanged(mode);
			}
		}

		public static void PartStateChanged(PartBehaviour part, StateType state)
		{
			if (EventHandlers.OnChangePartState != null)
			{
				EventHandlers.OnChangePartState(part, state);
			}
		}

		public static void PreviewCreated(PartBehaviour part)
		{
			if (EventHandlers.OnPreviewCreated != null)
			{
				EventHandlers.OnPreviewCreated(part);
			}
		}

		public static void PreviewCanceled(PartBehaviour part)
		{
			if (EventHandlers.OnPreviewCanceled != null)
			{
				EventHandlers.OnPreviewCanceled(part);
			}
		}

		public static void PlacedPart(PartBehaviour part, SocketBehaviour socket)
		{
			if (EventHandlers.OnPlacedPart != null)
			{
				EventHandlers.OnPlacedPart(part, socket);
			}
		}

		public static void WantPlacedPart(PartBehaviour part, SocketBehaviour socket)
		{
			if (EventHandlers.OnWantPlacedPart != null)
			{
				EventHandlers.OnWantPlacedPart(part, socket);
			}
		}

		public static void WantDestoryBuilding(long insId)
		{
			if (EventHandlers.OnWantDestoryBuilding != null)
			{
				EventHandlers.OnWantDestoryBuilding(insId);
			}
		}

		public static void ChangedAppearance(PartBehaviour part, int appearanceIndex)
		{
			if (EventHandlers.OnChangedAppearance != null)
			{
				EventHandlers.OnChangedAppearance(part, appearanceIndex);
			}
		}

		public static void DestroyedPart(PartBehaviour part)
		{
			if (EventHandlers.OnDestroyedPart != null)
			{
				EventHandlers.OnDestroyedPart(part);
			}
		}

		public static void EditedPart(PartBehaviour part, SocketBehaviour socket)
		{
			if (EventHandlers.OnEditedPart != null)
			{
				EventHandlers.OnEditedPart(part, socket);
			}
		}

		public static void StorageSaving()
		{
			if (EventHandlers.OnStorageSaving != null)
			{
				EventHandlers.OnStorageSaving();
			}
		}

		public static void StorageLoading()
		{
			if (EventHandlers.OnStorageLoading != null)
			{
				EventHandlers.OnStorageLoading();
			}
		}

		public static void StorageLoadingDone(PartBehaviour[] Parts)
		{
			if (EventHandlers.OnStorageLoadingDone != null)
			{
				EventHandlers.OnStorageLoadingDone(Parts);
			}
		}

		public static void StorageSavingDone(PartBehaviour[] Parts)
		{
			if (EventHandlers.OnStorageSavingDone != null)
			{
				EventHandlers.OnStorageSavingDone(Parts);
			}
		}

		public static void StorageFailed(string exception)
		{
			if (EventHandlers.OnStorageFailed != null)
			{
				EventHandlers.OnStorageFailed(exception);
			}
		}

		public static void StorageDeleted()
		{
			if (EventHandlers.OnStorageDeleted != null)
			{
				EventHandlers.OnStorageDeleted();
			}
		}
	}
}
