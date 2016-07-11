# Herald_UWP

* 直接Pull到本地可能还需要安装一些Reference。步骤：  
  1.Menu -> Tools -> NuGet Package Manager -> Package Manager Console  
  2.PM> Install-Package Microsoft.NETCore.UniversalWindowsPlatform
  
* 运行Windows Mobile的虚拟机需要Hyper-V的支持，Hyper-V需要Win10 Professional的支持。  
  另外默认Hyper-V是不开启的，需要在“程序和功能” -> “启用和关闭Windows功能”里面勾选

* 如果打开了Hyper-V，那么其它虚拟化的软件例如VMware，就不能用了（Interesting）。  
  而且Hyper-V的打开和关闭必须通过Windows启动时完成，所以只能用增加启动项的方式使这两种状态共存。  
  具体方法可参考： [设置Hyper-V和VMware多个服务之间共存](http://www.cnblogs.com/LonelyShadow/p/4152474.html)
