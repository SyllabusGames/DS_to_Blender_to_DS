using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.UI;
using System.Web.Script.Serialization;


using SoulsFormats;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using ObjLoader.Loader.Loaders;

using Assimp;
using System.Data;

using System.Threading;

namespace MySFformat {
	class ImportExport{

		static LogWindow log;

		
		public static void OpenDAEExportDialogue(FLVER model)
		{
			/*var importer = new AssimpContext();
			 Scene md = importer.ImportFile("SampleFile.dae", PostProcessSteps.CalculateTangentSpace);// PostProcessPreset.TargetRealTimeMaximumQuality



			 MessageBox.Show("Meshes count:" + md.Meshes.Count + "Material count:" + md.MaterialCount + "\nRootNode:" + md.RootNode.MeshIndices.Count);

			 string status = "RNode children count " + md.RootNode.ChildCount;

			 foreach (var vc in md.RootNode.Children)
			 {
				 status += "\n" + vc.Name + "- mesh count" + vc.MeshCount + "- has children" + vc.HasChildren;
				 if (vc.MeshCount > 0) { status += "- index:" + vc.MeshIndices[0]; }
			 }

			 MessageBox.Show(status);*/
			 
			var openFileDialog2 = new SaveFileDialog();
			openFileDialog2.FileName = "Exported.dae";
			if (openFileDialog2.ShowDialog() == DialogResult.OK)
			{
				ExportModel(model , openFileDialog2.FileName , FileTypes.collada);
			} else {
				return;
			}
		}

		public static void ExportModel(FLVER model , string filePathNoExtension , FileTypes type){
			try
			{

				Assimp.Scene scene = new Scene();
				scene.RootNode = new Node();


				//s.Meshes = new List<Mesh>();


				/*
				Assimp.Mesh m = new Mesh("Test triangles",Assimp.PrimitiveType.Triangle);
				m.Vertices.Add(new Assimp.Vector3D(1,0,0));
				m.Vertices.Add(new Assimp.Vector3D(0, 1, 0));
				m.Vertices.Add(new Assimp.Vector3D(0, 0, 1));
				   
					
				m.Normals.Add(new Assimp.Vector3D(0,1,0));
				m.Normals.Add(new Assimp.Vector3D(0, 1, 0));
				m.Normals.Add(new Assimp.Vector3D(0, 1, 0));
				m.Faces.Add(new Face(new int[] { 0,1,2}));
				m.MaterialIndex = 0;
				   
				   
				s.Meshes.Add(m);

				s.Materials.Add(new Material());

				s.RootNode = new Node();

				Node nbase = new Node();
				nbase.Name = "MeshName";
				nbase.MeshIndices.Add(0);

				s.RootNode.Children.Add(nbase);*/
				  
				for (int i = 0; i < model.Materials.Count; i++)
				{
					var m = model.Materials[i];

					var assimpMaterial = new Assimp.Material();
					assimpMaterial.Name = m.Name;
					scene.Materials.Add(assimpMaterial);
				}

				for (int i = 0; i < model.Meshes.Count; i++)
				{
					FLVER.Mesh sourceMesh = model.Meshes[i];
					Assimp.Mesh exportMesh = new Mesh("Mesh_M" + i, Assimp.PrimitiveType.Triangle);
				//	meshNew.VertexColorChannels = new List<Color4D>[m.Vertices[0].Colors.Count];
					foreach (var v in sourceMesh.Vertices)
					{
						exportMesh.Vertices.Add(new Assimp.Vector3D(v.Positions[0].X, v.Positions[0].Y, v.Positions[0].Z));
						exportMesh.Normals.Add(new Assimp.Vector3D(v.Normals[0].X, v.Normals[0].Y, v.Normals[0].Z));
						exportMesh.Tangents.Add(new Assimp.Vector3D(v.Tangents[0].X, v.Tangents[0].Y, v.Tangents[0].Z));
				//		meshNew.VertexColorChannels[0].Add(new Assimp.Vector3D(v.Colors[0].X, v.Tangents[0].Y, v.Tangents[0].Z));
						for(int uv = 0 ; uv < v.UVs.Count ; uv++){
							exportMesh.TextureCoordinateChannels[uv].Add(new Assimp.Vector3D(v.UVs[uv].X,1 - v.UVs[uv].Y,0));
						}
			//			Console.WriteLine("UV Channel Count: " + v.UVs.Count);
					}
						
						
					foreach (var fs in sourceMesh.FaceSets)
					{
						foreach (var arr in fs.GetFaces())
						{
							exportMesh.Faces.Add(new Face(new int[] { (int)arr[0], (int)arr[1],(int)arr[2] }));
						}
					}

					exportMesh.MaterialIndex = sourceMesh.MaterialIndex;
					scene.Meshes.Add(exportMesh);


					Node nbase = new Node();
					nbase.Name = "M_" + i + "_" + model.Materials[sourceMesh.MaterialIndex].Name;
					nbase.MeshIndices.Add(i);

					scene.RootNode.Children.Add(nbase);

				}


				using(AssimpContext exportor = new AssimpContext()){
					exportor.ExportFile(scene, filePathNoExtension + ".dae", "collada");
					switch(type){
						case FileTypes.collada:filePathNoExtension += ".dae";break;
						case FileTypes.gltf2:filePathNoExtension += ".gltf";break;
						default: filePathNoExtension += "." + type.ToString();break;
					}
					exportor.ExportFile(scene, filePathNoExtension, type.ToString() , PostProcessSteps.FlipWindingOrder);
					exportor.Dispose();
				}
	//			MessageBox.Show("Export successful!", "Info");

			}
			catch (Exception ex)
			{
				MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
				$"Details:\n\n{ex.StackTrace}");
			}














			{
				Assimp.Scene scene = new Scene();
				scene.RootNode = new Node();

				var assimpMaterial = new Assimp.Material();
				assimpMaterial.Name = "Test";
				scene.Materials.Add(assimpMaterial);

				Assimp.Mesh exportMesh = new Mesh("TestMesh" , Assimp.PrimitiveType.Triangle);
				exportMesh.Vertices.Add(new Assimp.Vector3D(0 , 0 , 0));
				exportMesh.Vertices.Add(new Assimp.Vector3D(1 , 0 , 0));
				exportMesh.Vertices.Add(new Assimp.Vector3D(0 , 1 , 0));
				exportMesh.Normals.Add(new Assimp.Vector3D(0,0,1));
				exportMesh.Normals.Add(new Assimp.Vector3D(0,0,1));
				exportMesh.Normals.Add(new Assimp.Vector3D(0,0,1));
				exportMesh.Tangents.Add(new Assimp.Vector3D(1,0,0));
				exportMesh.Tangents.Add(new Assimp.Vector3D(1,0,0));
				exportMesh.Tangents.Add(new Assimp.Vector3D(1,0,0));
				exportMesh.TextureCoordinateChannels[0].Add(new Assimp.Vector3D(0,0,0));
				exportMesh.TextureCoordinateChannels[0].Add(new Assimp.Vector3D(1,0,0));
				exportMesh.TextureCoordinateChannels[0].Add(new Assimp.Vector3D(0,1,0));
				
				
				exportMesh.Faces.Add(new Face(new int[] {0,1,2}));

				exportMesh.MaterialIndex = 0;
				scene.Meshes.Add(exportMesh);


				Node nbase = new Node();
				nbase.Name = "M_" + "Material_Name";
				nbase.MeshIndices.Add(0);

				scene.RootNode.Children.Add(nbase);
				
				using(AssimpContext exportor = new AssimpContext()){
					exportor.ExportFile(scene, @"C:/Users/nsrul/Desktop/DS To Blender/tri.dae", "collada");
					exportor.ExportFile(scene, @"C:/Users/nsrul/Desktop/DS To Blender/tri.gltf" , "gltf2" , PostProcessSteps.FlipWindingOrder);
					exportor.Dispose();
				}
			}


		}

//		I am uisng the c# Assimp library to export a mesh to .gltf. While the mesh exports properly, it does not export UV coordinates. The Assimp.Mesh object has the us coordinates loaded properly because if I export to a collada .dae file, the UVs are exported properly. What could cause the uvs to not export specifically when saving at .gltf file?
		
		public static void GLTFToBlend(){
			// Specify the paths to Blender and your Python script
			string blenderPath = @"C:/Program Files/Blender 3.0/blender.exe";
			string defaultBlendFilePath = @"C:/Users/nsrul/Desktop/DS To Blender/empty.blend";

			string[] gltfFilePaths = new string[]{"C:/Users/nsrul/Desktop/DS To Blender/exported.gltf"};
			string outputBlenderFile = "C:/Users/nsrul/Desktop/DS To Blender/ExportedModel.blend";

			//		YEP That's a tripple escaped " symbol. (\\\") The fist and third escape characters are stripped off by C#, meaning the console sees it as \" and it is just a " when Python gets it. This is so stupid.

			string pythonCode = "import bpy;";
			foreach(string file in gltfFilePaths){
				pythonCode += "bpy.ops.import_scene.gltf(filepath=\\\"" + file + "\\\");";
			}
			pythonCode += "bpy.ops.wm.save_mainfile(filepath=\\\"" + outputBlenderFile + "\\\");";
			Console.WriteLine(pythonCode);

	//		pythonCode = "import bpy;bpy.ops.import_scene.gltf(filepath=\"C:/Users/nsrul/Desktop/DS To Blender/exported.gltf\");bpy.ops.wm.save_mainfile(filepath=\"C:/Users/nsrul/Desktop/DS To Blender/ExportedModel.blend\");";
			Console.WriteLine(pythonCode);

/*
//		I manually copied these into a Command Prompt to test them as I figured out how this all works
"C:/Program Files/Blender 3.0/blender.exe" "C:/Users/nsrul/Desktop/DS To Blender/empty.blend" --python "C:/Users/nsrul/Desktop/DS To Blender/saveBlend.py"
"C:/Program Files/Blender 3.0/blender.exe" "C:/Users/nsrul/Desktop/DS To Blender/empty.blend" --python "C:/Users/nsrul/Desktop/DS To Blender/print.py"
"C:/Program Files/Blender 3.0/blender.exe" "C:/Users/nsrul/Desktop/DS To Blender/empty.blend" --python-expr "print(5);print(6);"
"C:/Program Files/Blender 3.0/blender.exe" "C:/Users/nsrul/Desktop/DS To Blender/empty.blend" --python-expr "import bpy;print(5);print(6);"
"C:/Program Files/Blender 3.0/blender.exe" "C:/Users/nsrul/Desktop/DS To Blender/empty.blend" --python-expr "import bpy;print(5);bpy.ops.import_scene.gltf(filepath=\"C:/Users/nsrul/Desktop/DS To Blender/exported.gltf\");"
"C:/Program Files/Blender 3.0/blender.exe" "C:/Users/nsrul/Desktop/DS To Blender/empty.blend" --python-expr "import bpy;bpy.ops.import_scene.gltf(filepath=\"C:/Users/nsrul/Desktop/DS To Blender/exported.gltf\");bpy.ops.wm.save_mainfile(filepath=\"C:/Users/nsrul/Desktop/DS To Blender/ExportedModel.blend\");"
"C:/Program Files/Blender 3.0/blender.exe" --background "C:/Users/nsrul/Desktop/DS To Blender/empty.blend" --python-expr "import bpy;bpy.ops.import_scene.gltf(filepath=\"C:/Users/nsrul/Desktop/DS To Blender/exported.gltf\");bpy.ops.wm.save_mainfile(filepath=\"C:/Users/nsrul/Desktop/DS To Blender/ExportedModel.blend\");"

"C:/Program Files/Blender 3.0/blender.exe" --python-expr "import bpy;bpy.ops.preferences.addon_disable(module='io_soulstruct');bpy.ops.wm.quit_blender();"
*/


			try
			{
		//		Process blender = Process.Start(blenderPath, "\"C:/Users/nsrul/Desktop/DS To Blender/empty.blend\"");//		Works
		//		Process blender = Process.Start(blenderPath, "\"C:/Users/nsrul/Desktop/DS To Blender/empty.blend\" --python-expr \"print(5);\"");
		//		Process blender = Process.Start(blenderPath, "\"C:/Users/nsrul/Desktop/DS To Blender/empty.blend\" --python-expr \"import bpy;print(5);\"");
		//		Process blender = Process.Start(blenderPath, "\"C:/Users/nsrul/Desktop/DS To Blender/empty.blend\" --python-expr \"import bpy;print(5);bpy.ops.import_scene.gltf(filepath=\\\"C:/Users/nsrul/Desktop/DS To Blender/exported.gltf\\\");bpy.ops.wm.save_mainfile(filepath=\\\"C:/Users/nsrul/Desktop/DS To Blender/ExportedModel.blend\\\");\"");
				Process blender = Process.Start(blenderPath, $"--background \"{defaultBlendFilePath}\" --python-expr \"{pythonCode}\" --exit-code 1");
			//	Process blender = Process.Start(blenderPath, $"--background \"{blendFilePath}\" --python \"{pythonScriptPath}\" --exit-code 1");
			//	SendKeys.SendWait("^v");
	//			blender.WaitForExit();
				Console.WriteLine("Blender script execution completed successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error executing Blender script: {ex.Message}");
			}
		}


		static void LogStuffThread()
		{
			int number = 0;
			while (true)
			{
		//		listBoxLog.Log(Level.Info, "A info level message from thread # {0,0000}", number++);
				Thread.Sleep(2000);
			}
		}



		/*
		//		Since Blender is ?Left? handed (not the same as DS anyway) I have to flip all vertices and reverse triangle orders
		static unsafe void ExportFlverToFile(FLVER flver , string map , string name, string assetName , FileTypes type , string texturePath = null, bool mapflver = false){
			Assimp.Scene scene = new Assimp.Scene();
			scene.RootNode = new Assimp.Node("Root");
			switch(type){
				case FileTypes.collada:
					break;
				case FileTypes.obj:
					break;
				case FileTypes.gltf2:
					break;
			}

			int meshNumber = 0;
			foreach(var m in flver.Meshes){
				var verts = new List<Vector3>();
				var vertsAssimp = new List<Assimp.Vector3D>();
				var normals = new List<Vector3>();
				var normalsAssimp = new List<Assimp.Vector3D>();
				var tangents = new List<Vector4>();
				int uvcount = m.Vertices[0].UVCount;
				List<Vector2>[] uvs = new List<Vector2>[uvcount];
				List<Material> matList = new List<Material>();

				Assimp.Mesh mesh = new Assimp.Mesh("Mesh_" + meshNumber);

				int lightmapUVIndex = 1;
			
				for (int i = 0; i < uvs.Length; i++){
					uvs[i] = new List<Vector2>();
				}
				foreach (var v in m.Vertices){
					verts.Add(new Vector3(v.Positions[0].X, v.Positions[0].Y, -v.Positions[0].Z));//		Invert the Z coordinate (This will be the Y coordinate in Blender)
					vertsAssimp.Add(new Assimp.Vector3D(v.Positions[0].X, v.Positions[0].Y, -v.Positions[0].Z));//		Invert the Z coordinate (This will be the Y coordinate in Blender)
					normals.Add(new Vector3(v.Normals[0].X, v.Normals[0].Y, v.Normals[0].Z));
					normalsAssimp.Add(new Assimp.Vector3D(v.Normals[0].X, v.Normals[0].Y, v.Normals[0].Z));
				/*	if (v.Tangents.Count > 0)
					{
						tangents.Add(new Vector4(v.Tangents[0], v.Tangents[1], v.Tangents[2], v.Tangents[3]));
					}
					else
					{
						tangents.Add(new Vector4(0, 0, 0, 1));
					}*
					for (int i = 0; i < uvs.Length; i++){
						// Swap lightmap uvs with uv index 1 because lmao unity
						if (i == 1){
							uvs[i].Add(new Vector2(v.UVs[lightmapUVIndex*3+0].x, 1.0f - v.UVs[lightmapUVIndex*3+1].));
						}
						else if (i == lightmapUVIndex){
							uvs[i].Add(new Vector2(v.UVs[1*3], 1.0f - v.UVs[1*3+1]));
						}else{
							uvs[i].Add(new Vector2(v.UVs[i*3], 1.0f - v.UVs[i*3+1]));
						}
					}
				}

				List<int> triangles = new List<int>();
				foreach (var fs in m.FaceSets){
					if (fs.Indices.Count() == 0)
						continue;
					if (fs.Flags != FLVER.FaceSet.FSFlags.None)
						continue;
					triangles.AddRange(fs.GetFacesArray());
				}

				mesh.Vertices.AddRange(vertsAssimp);
				mesh.SetIndices(triangles.ToArray() , 3);
				mesh.Normals.AddRange(normalsAssimp);
			//	mesh.Faces.Add(new Assimp.Face(new int[] { 0, 1, 2 })); 
				mesh.MaterialIndex = meshNumber; 
  
				scene.Meshes.Add(mesh); 
				scene.RootNode.MeshIndices.Add(meshNumber); 
  
				Assimp.Material mat = new Assimp.Material(); 
				mat.Name = "Material_" + meshNumber; 
				scene.Materials.Add(mat); 

				/*
				Assimp.Node obj = new Assimp.Node(mesh.Name );//		Link the mesh to an object
				obj.Transform = Assimp.Matrix4x4.Identity;
				obj.MeshIndices.Add(scene.Meshes.Count);
				scene.Meshes.Add(mesh);
				scene.RootNode.Children.Add(obj);//		Link the object to the scene
				*

				meshNumber++;
			}

			Assimp.AssimpContext context = new Assimp.AssimpContext(); 
			//		These all work
			context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\DS To Blender\OldScript "+name+".gltf", "gltf2");
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".gltf", "gltf");//		gltf 1.0
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".glb", "glb");//		gltf 1.0
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".dae", "collada");
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".obj", "obj");
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".stl", "stl");
			//		These all work

			//		These export, but I can't check if the model is correct.
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".3ds", "3ds");
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".assbin", "assbin");
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".ply", "ply");
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".x", "x");
			//		These don't seem to work
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".json", "json");
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".step", "step");
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".pbrtv4", "pbrtv4");
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".3MF", "3MF");
		//	context.ExportFile(scene, $@"C:\Users\nsrul\Desktop\gltf\"+name+".fbx", "fbx");
		}*/
	}
}


/*
//	Chat GPT
I am using c# to launch Blender through the command line through System.Diagnostics.Process. how can I use the --python-text command to run the following code:

*/