﻿/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using UnityEditor;
using System.IO;
using CatLib.API.AssetBuilder;
using CatLib.API;
using CatLib.API.IO;

namespace CatLib.AssetBuilder
{

    public class PrecompiledStrategy : IBuildStrategy
    {


        [Dependency]
        public IEnv Env { get; set; }

        public BuildProcess Process { get { return BuildProcess.Precompiled; } }

        public void Build(IBuildContext context)
        {

            BuildAssetBundleName(context);

        }

        /// <summary>
        /// 编译AssetBundle标记的名字
        /// </summary>
        protected void BuildAssetBundleName(IBuildContext context)
        {

            IDirectory directory = context.Disk.Directory(context.BuildPath, PathTypes.Absolute);
            directory.Walk((file) => {

                if (!file.Name.EndsWith(".meta"))
                {
                    BuildFileBundleName(file, context.BuildPath);
                }

            }, SearchOption.AllDirectories);

        }

        /// <summary>
        /// 编译文件AssetBundle名字
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <param name="basePath">基础路径</param>

        protected void BuildFileBundleName(IFile file, string basePath)
        {

            string extension = file.Extension;
            string fullName = file.FullName.Standard();
            string fileName = file.Name;
            string baseFileName = fileName.Substring(0, fileName.Length - extension.Length);
            string assetName = fullName.Substring(basePath.Length);
            assetName = assetName.Substring(0, assetName.Length - fileName.Length).TrimEnd(Path.AltDirectorySeparatorChar);

            if (baseFileName + extension == ".DS_Store") { return; }

            int variantIndex = baseFileName.LastIndexOf(".");
            string variantName = string.Empty;

            if (variantIndex > 0)
            {

                variantName = baseFileName.Substring(variantIndex + 1);

            }

            AssetImporter assetImporter = AssetImporter.GetAtPath("Assets" + Env.ResourcesBuildPath + assetName + Path.AltDirectorySeparatorChar + baseFileName + extension);
            assetImporter.assetBundleName = assetName.TrimStart(Path.AltDirectorySeparatorChar);
            if (variantName != string.Empty)
            {

                assetImporter.assetBundleVariant = variantName;

            }

        }



    }

}