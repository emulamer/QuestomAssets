using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class QueuedFileOp : AssetOp
    {
        public QueuedFileOp()
        {

        }

        public override bool IsWriteOp => true;

        public QueuedFileOperationProviderType ProviderType { get; set;}


        public QueuedFileOperationType Type { get; set; }

        /// <summary>
        /// For CopyFile the file to copy from
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// For ExtractZipToFolder the path to extract the zip to
        /// For WriteFile CopyFile and DeleteFile the target filename
        /// For DeleteFolder the directory to delete
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// For ExtractZipToFolder the bytes of the zip file
        /// For WriteFile the bytes of the file
        /// </summary>
        public byte[] SourceData { get; set; }

        internal void PerformFileOperation(QaeConfig config)
        {
            try
            {
                IFileProvider provider;
                switch (ProviderType)
                {
                    case QueuedFileOperationProviderType.Root:
                        provider = config.RootFileProvider;
                        break;
                    case QueuedFileOperationProviderType.ModLibs:
                        provider = config.ModLibsFileProvider;
                        break;
                    default:
                        throw new NotImplementedException($"Provider type {ProviderType} is not implemented in QueuedFileOp.");
                }

                switch (Type)
                {
                    case QueuedFileOperationType.DeleteFile:
                        if (!provider.FileExists(TargetPath))
                        {
                            Log.LogErr($"Queued file operation was supposed to delete '{TargetPath}' but it didn't exist!");
                        }
                        else
                        {
                            provider.Delete(TargetPath);
                        }
                        break;
                    case QueuedFileOperationType.DeleteFolder:
                        if (!provider.DirectoryExists(TargetPath))
                        {
                            Log.LogErr($"Queued file operation was supposed to delete '{TargetPath}' but it didn't exist!");
                        }
                        else
                        {
                            provider.RmRfDir(TargetPath);
                        }
                        break;
                    case QueuedFileOperationType.ExtractZipToFolder:
                        throw new NotImplementedException();
                    case QueuedFileOperationType.WriteFile:
                        provider.MkDir(TargetPath.GetDirectoryFwdSlash(), true);
                        provider.Write(TargetPath, SourceData, true);
                        break;
                }
            }
            catch (NotImplementedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.LogErr($"Exception handling queued file operation of type {Type}", ex);
                throw;
            }
        }

        internal override void PerformOp(OpContext context)
        {
            context.Engine.QueuedFileOperations.Add(this);
        }
    }

    public enum QueuedFileOperationType
    {
        DeleteFile,
        DeleteFolder,
        WriteFile,
        ExtractZipToFolder
    }

    public enum QueuedFileOperationProviderType
    {
        Root,
        ModLibs
    }
}
