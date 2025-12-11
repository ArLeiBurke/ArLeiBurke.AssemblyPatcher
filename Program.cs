using Mono.Cecil;

namespace ArLeiBurke.AssemblyPatcher
{
	public class Program
	{
		static void Main(string[] args)
		{

			// C++ dll对应的原名 和修改后的名字！！！！
			string halcondllName = "halcon";
			string halcondllChangedName = "ArLeiBurke.ImageProcessing";

			// C# dll 对应的原名及修改后的名字！！！
			string halcondotnetName = "halcondotnet.dll";
			string halcondotnetChangedName = "ArLeiBurke.ImageProcessingDotNet.dll";

			var asm = AssemblyDefinition.ReadAssembly(@"C:\Users\l\Desktop\halcondotnet.dll", new ReaderParameters { ReadWrite = true });

			// 下面这行代码用来判断 C#编写的程序集的强签名是否还存在！！！  true 表示 强签名还在 
			bool isStrongNameSigned = (asm.MainModule.Attributes & ModuleAttributes.StrongNameSigned) != 0;

			asm.Name = new AssemblyNameDefinition("ArLeiBurke.ImageProcessingDotNet", new Version(22, 5, 0, 0));
			//asm.MainModule.Name = halcondotnetChangedName;

			// 下面这行代码非常重要，，一定要去掉，，要不然  在 Visual Studio 里面引用新生成的程序集的时候是会给我报错的，，报 The reference is invalid or unsupported!!!!
			asm.MainModule.Attributes &= ~ModuleAttributes.StrongNameSigned;

			// 下面这个 foreach 是用来去除掉 		[DllImport("halcon", CallingConvention = CallingConvention.Cdecl)]  中的 halcon ，并且用 XXX来代替，，XXX 指的是对应的C++ dll 的名字,我这里是 ArLeiBurke.ImageProcessing
			foreach (var module in asm.Modules)
			{
				foreach (var type in module.Types)
				{
					foreach (var method in type.Methods)
					{
						// 判断方法是否是 P/Invoke 方法。 
						if (method.HasPInvokeInfo)
						{
							var Pinvoke = method.PInvokeInfo;

							if (Pinvoke.Module.Name == halcondllName)
							{
								// 往 Assembly 里面添加新的引用！！！
								var NewModuleReference = module.ModuleReferences.FirstOrDefault(mr => mr.Name == halcondllChangedName);
								if (NewModuleReference == null)
								{
									NewModuleReference = new ModuleReference(halcondllChangedName);
									module.ModuleReferences.Add(NewModuleReference);
								}

								// 创建新的 PInvokeInfo
								var NewPinvoke = new PInvokeInfo(
									Pinvoke.Attributes,
									Pinvoke.EntryPoint,
									NewModuleReference
								);

								method.PInvokeInfo = NewPinvoke;
								Console.WriteLine($"Method: {method.Name}, Dll changed to: {method.PInvokeInfo.Module.Name}");
							}
						}
					}
				}


				// 移除 Assembly 里面原有的 halcon 引用！！！！      下面这几行代码一定要最后删！！！因为  method.PInvokeInfo 内部其实是一个 指向 ModuleReference 的引用： 如果我提前把 halcon 引用删除掉的话，，会导致 var Pinvoke = method.PInvokeInfo; 这行代码一直返回null！！！
				var HalconReference = module.ModuleReferences.FirstOrDefault(mr => mr.Name == "halcon");
				if (HalconReference != null)
					module.ModuleReferences.Remove(HalconReference);


				// 将修改后的 Assembly 保存在本地！！！
				asm.Write("ArLeiBurke.ImageProcessingDotNet.dll", new WriterParameters
				{
					StrongNameKeyPair = null
				});


			}
		}

		// Verify whether the assembly is usable after patching.
		private static void Test()
		{
			//HImage image = new HImage();
			//image.ReadImage("temp.bmp");
			//Console.WriteLine("Hello, World!");
			//Console.ReadLine();
			//return;

		}
	}
}
