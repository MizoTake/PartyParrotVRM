using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PartyParrotVRM.Controller;
using UnityEngine;
using VRM;

namespace PartyParrotVRM.Provider
{
    public class VrmProvider : MonoBehaviour
    {

        private AvatarController avatarController;
        private int length;
        private int index;
        private List<byte> byteContent = new List<byte>();
        
        public void Injection(AvatarController avatarController)
        {
            this.avatarController = avatarController;
        }

#if UNITY_EDITOR
        private void Start()
        {
            // using (var reader = new FileStream($"{Define.MOCK_JSON_DIRECTORY}/sample/sample.vrm", FileMode.Open))
            // {
            //     var data = new byte[reader.Length];
            //     reader.Read(data, 0, data.Length);
            //     byteContent.AddRange(data);
            //     UpdateVrm();
            //     reader.Close();
            // }
        }
#endif

        public void ByteLength(int length)
        {
            this.length = length;
            index = 0;
            byteContent.Clear();
        }

        public void AddRange(string byteString)
        {
            var decode = Convert.FromBase64String(byteString);
            byteContent.AddRange(decode);
            index += 1;
            if (length != index) return;
            UpdateVrm();
        }
        
        public void UpdateVrm()
        {
            var vrmBytes = byteContent.ToArray();
            var context = new VRMImporterContext();
            context.ParseGlb(vrmBytes);
            context.Load();
            context.ShowMeshes();
            context.EnableUpdateWhenOffscreen();
            avatarController.ChangeAvatar(context.Root);
        }
    }
}
