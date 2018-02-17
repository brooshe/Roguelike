using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LAYER = Property.Room.Layer;

namespace UI
{
    public class Minimap
    {
        private MinimapImage minimapImgBasement;
        private MinimapImage minimapImgGround;
        private MinimapImage minimapImgUpstairs;
        private RawImage mainCharImg;
        private CharacterPawn mainPawn;
        private Vector2 bkgCenter;

        LAYER curLayer;

        public void Init(MinimapImage basement, MinimapImage ground, MinimapImage upstairs, RawImage charImg)
        {
            minimapImgBasement = basement;
            minimapImgGround = ground;
            minimapImgUpstairs = upstairs;
            mainCharImg = charImg;
            bkgCenter = basement.rectTransform.offsetMin + basement.rectTransform.offsetMax;
            bkgCenter /= 2;

            SetLayer(LAYER.GROUND);
        }

        public void SyncPawn(CharacterPawn pawn)
        {
            Debug.Log("SyncPawn");
            mainPawn = pawn;
            if (mainPawn.curRoom != null)
            {
                IntVector3 logicPos = mainPawn.curRoom.LogicPosition;
                LAYER layer = logicPos.y == -1 ? LAYER.BASEMENT : (logicPos.y == 0 ? LAYER.GROUND : LAYER.UPSTAIRS);
                SetLayer(layer);
            }
        }

        public void AddRoom(IntVector3 roomPos)
        {
            MinimapImage img = null;
            switch(roomPos.y)
            {
                case -1:
                    img = minimapImgBasement;
                    break;
                case 0:
                    img = minimapImgGround;
                    break;
                case 1:
                    img = minimapImgUpstairs;
                    break;
            }
            if (img == null)
                return;

            //minimap.ClearQuad();
            Rect r = img.GetPixelAdjustedRect();
            Vector2 center = r.center;
            center.x += roomPos.x * 20;
            center.y += roomPos.z * 20;
            center -= new Vector2(10, 10);
            Rect pos = new Rect(center, new Vector2(19, 19));
            img.AddQuad(pos, new Rect(0, 0, 1, 1));
            img.SetMeshDirty();
        }

        public void Tick(float deltaTime)
        {
            if(mainPawn != null)
            {
                IntVector3 logicPos = mainPawn.curRoom.LogicPosition;
                Vector3 pos = new Vector3(bkgCenter.x + logicPos.x * 20, bkgCenter.y + logicPos.z * 20, 0);
                LAYER layer = logicPos.y == -1 ? LAYER.BASEMENT : (logicPos.y == 0 ? LAYER.GROUND : LAYER.UPSTAIRS);
                SetLayer(layer);

                Quaternion pawnRot = mainPawn.transform.rotation;
                Quaternion logicRot = mainPawn.curRoom.WorldToLogic(pawnRot);
                float yaw = logicRot.eulerAngles.y;
                RectTransform tran = mainCharImg.rectTransform;
                Vector3 euler = tran.rotation.eulerAngles;
                euler.z = -yaw;

                tran.localPosition = pos;
                tran.rotation = Quaternion.Euler(euler);
                //tran.SetPositionAndRotation(pos, Quaternion.Euler(euler));
            }
        }

        void SetLayer(LAYER layer)
        {
            if (layer == curLayer)
                return;
            curLayer = layer;
            switch(curLayer)
            {
                case LAYER.BASEMENT:
                    minimapImgBasement.enabled = true;
                    minimapImgGround.enabled = false;
                    minimapImgUpstairs.enabled = false;
                    break;
                case LAYER.GROUND:
                    minimapImgBasement.enabled = false;
                    minimapImgGround.enabled = true;
                    minimapImgUpstairs.enabled = false;
                    break;
                case LAYER.UPSTAIRS:
                    minimapImgBasement.enabled = false;
                    minimapImgGround.enabled = false;
                    minimapImgUpstairs.enabled = true;
                    break;
            }
        }

        public void Dispose()
        {
            minimapImgBasement = null;
            minimapImgGround = null;
            minimapImgUpstairs = null;
            mainCharImg = null;
            mainPawn = null;
        }
    }
}
