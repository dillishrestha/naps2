﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using NAPS2.Images;
using NAPS2.ImportExport.Email;
using NAPS2.ImportExport.Email.Mapi;
using NAPS2.Scan;
using NAPS2.Scan.Wia;
using NAPS2.Serialization;

namespace NAPS2.Remoting.Worker
{
    public class WorkerServiceAdapter
    {
        private readonly WorkerService.WorkerServiceClient client;

        public WorkerServiceAdapter(int port, ChannelCredentials creds)
        {
            client = new WorkerService.WorkerServiceClient(new Channel("localhost", port, creds));
        }

        public void Init(string recoveryFolderPath)
        {
            var req = new InitRequest { RecoveryFolderPath = recoveryFolderPath ?? "" };
            var resp = client.Init(req);
            RemotingHelper.HandleErrors(resp.Error);
        }

        public WiaConfiguration Wia10NativeUI(string scanDevice, IntPtr hwnd)
        {
            var req = new Wia10NativeUiRequest
            {
                DeviceId = scanDevice,
                Hwnd = (ulong)hwnd
            };
            var resp = client.Wia10NativeUi(req);
            RemotingHelper.HandleErrors(resp.Error);
            return resp.WiaConfigurationXml.FromXml<WiaConfiguration>();
        }

        public List<ScanDevice> TwainGetDeviceList(TwainImpl twainImpl)
        {
            var req = new TwainGetDeviceListRequest { TwainImpl = twainImpl.ToXml() };
            var resp = client.TwainGetDeviceList(req);
            RemotingHelper.HandleErrors(resp.Error);
            return resp.DeviceListXml.FromXml<List<ScanDevice>>();
        }

        public async Task TwainScan(ScanDevice scanDevice, ScanProfile scanProfile, ScanParams scanParams, IntPtr hwnd, CancellationToken cancelToken, Action<ScannedImage, string> imageCallback)
        {
            var req = new TwainScanRequest
            {
                ScanDeviceXml = scanDevice.ToXml(),
                ScanProfileXml = scanProfile.ToXml(),
                ScanParamsXml = scanParams.ToXml(),
                Hwnd = (ulong)hwnd
            };
            var streamingCall = client.TwainScan(req, cancellationToken: cancelToken);
            while (await streamingCall.ResponseStream.MoveNext())
            {
                var resp = streamingCall.ResponseStream.Current;
                RemotingHelper.HandleErrors(resp.Error);
                var scannedImage = SerializedImageHelper.Deserialize(resp.Image, new SerializedImageHelper.DeserializeOptions());
                imageCallback?.Invoke(scannedImage, resp.Image.RenderedFilePath);
            }
        }

        public MapiSendMailReturnCode SendMapiEmail(EmailMessage message)
        {
            var req = new SendMapiEmailRequest { EmailMessageXml = message.ToXml() };
            var resp = client.SendMapiEmail(req);
            RemotingHelper.HandleErrors(resp.Error);
            return resp.ReturnCodeXml.FromXml<MapiSendMailReturnCode>();
        }

        public byte[] RenderThumbnail(ScannedImage.Snapshot snapshot, int size)
        {
            var req = new RenderThumbnailRequest
            {
                Image = SerializedImageHelper.Serialize(snapshot, new SerializedImageHelper.SerializeOptions
                {
                    RequireFileStorage = true
                }),
                Size = size
            };
            var resp = client.RenderThumbnail(req);
            RemotingHelper.HandleErrors(resp.Error);
            return resp.Thumbnail.ToByteArray();
        }
    }
}