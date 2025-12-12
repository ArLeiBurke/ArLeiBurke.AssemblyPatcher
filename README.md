
## 背景
手边有两个dll，一个是用C#编写的 A.dll ，另一个是用C++编写的B.dll。<br>
B.dll 有导出好多函数供 A.dll 使用,通过P/Invoke 的方式。<br>
想要实现对这两个dll进行重命名以达到其他人看不到我有使用这两个dll 的目的！！<br>

## 实现步骤
1.直接对A.dll 重命名为 ChangedA.dll<br>
2.通过Mono.Cecil修改程序集的 AssemblyName <br>
3.去掉程序集的强签名<br>
4.修改 DllImport Attribute 的第一个参数<br>
5.将修改后的程序集保存在本地<br>

## 效果图
修改前的程序集<br>
<img width="435" height="180" alt="image" src="https://github.com/user-attachments/assets/0fd4088d-cc01-4df2-99f8-1c8a62a33a91" />

修改后的程序集<br>
<img width="294" height="189" alt="image" src="https://github.com/user-attachments/assets/396e8395-49f9-40fe-834f-25081af4a5fa" />

## 碎碎念
我拿改后的程序集简单测试了一下，Visual Studio 里面C#项目是能够正常引用 Patch 后的dll。用了几个常用的算子测试了一下，也没有给我报错。<br>
但是,如果项目里面有用到 HSmartWindowControlWPF 这个控件的话，build项目的时候会报错，还没有找到是因为什么原因导致的报错。<br>
下面的是报错截图<br>
<img width="1574" height="405" alt="image" src="https://github.com/user-attachments/assets/edc10ea9-a4fc-447e-838c-36b00b20106d" />
OK 基本情况就是这个样子！！下班！！！






