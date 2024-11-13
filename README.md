# Windows Sandbox Configuration Customization with pre-installed Visual Studio 2022 Community 
	-For information, I used the information in the Trace.txt file to see what the visual studio installer does.
	-I wanted to make it simple without using procmon via the "FileWatcher" program.	

## A- Pre-install visual studio 2022 community by modifying the base image BaseLayer.vhdx
1. use powershell or Hyper-manager to change the minimum image size from 8GB to 60GB or more
2. Stop Container Manager Service (services.msc)
3. Launch Hyper-manager and select the base image located in
   **C:\ProgramData\Microsoft\Windows\Containers\BaseImages\<GUID>\BaseLayer\BaseLayer.vhdx**
5. Do the same for the vhdx files (SnapshotSandbox.vhdx and SystemTemplate.vhdx) located in
  **C:\ProgramData\Microsoft\Windows\Containers\BaseImages\<GUID>\Snapshot**

## B- Install visual studio 2022 community to retrieve the various directories used to configure a base image (BaseLayer.vhx)
1. Start the "Container Manager Services" service (services.msc)
2. Define a share directory from the host, or define a mapping directory (VM lacing file *.wsb) writable from the VM.
3. Launch your VM via a configuration file "BaseImageVHDX_WithVisualStudio2022Community.wsb".
```
 <Configuration>
	<Networking>Enable</Networking>
	<ProtectedClient>Enable</ProtectedClient>
	<PrinterRedirection>Disable</PrinterRedirection>
	<ClipboardRedirection>Disable</ClipboardRedirection>
	<vGpu>Disable</vGpu>
	<MemoryInMB>10240</MemoryInMB>
		
	<MappedFolder>
	  <HostFolder>PATH_TO_YOUR_DIRECTORY_IN_WRITE</HostFolder>
	  <SandboxFolder>C:\Users\WDAGUtilityAccount\Downloads\sandbox_dumps</SandboxFolder>
	  <ReadOnly>false</ReadOnly>
	</MappedFolder>
<Configuration>	
```
	
**__INFO Download the visual studio 2022 community installation file from the microsoft website__**<br/>
**__INFO Install visual studio with the otpions you want (pay attention to the VM size you've defined, as an installation can easily be over 30GB)__**

## C- Copy the directories needed to run visual studio and prepare a pre-configured basic VM
1. Copy or compress the entire **"C:\Program Files"** directory into your shared directory or C:\Users\WDAGUtilityAccount\Downloads\sandbox_dumps
3. Copy or compress the entire **"C:\Program Files (x86)"** directory into your shared directory or C:\Users\WDAGUtilityAccount\Downloads\sandbox_dumps
4. Copy or compress the entire **"C:\ProgramData"** directory into your shared directory or C:\Users\WDAGUtilityAccount\Downloads\sandbox_dumps
5. Copy or compress the entire **C:\Users\WDAGUtilityAccount** directory into your shared directory or C:\Users\WDAGUtilityAccount\Downloads\sandbox_dumps.
7. Export the entire registry and copy it to your shared directory or C:\Users\WDAGUtilityAccount\Downloads\sandbox_dumps.
(for information, to be sure of copying all files and avoid copy permissions: change the owner of these different directories with WDAGUtilityAccount (directory property->security tab->advanced permissions).
8. Once all copies have been made, you can close the VM.

<br/><span style="color:red;">**__The directory WDAGUtilityAccount contains, among other things, the information required for the visual studio community license to be applied (otherwise you'll get the message “license expired”)__** </span>
	
## D- Preparing and modifying the base image (BaseLayer.vhdx)
1. Stop the Container Manager Service (services.msc)
2.  Mount the BaseLayer.vhdx image, located in C:\ProgramData\Microsoft\Windows\Containers\BaseImages\<GUID>\BaseLayer\BaseLayer.vhdx
3.  Copy the "Program Files", "Program Files (x86)" and "ProgramData" directories to "C:\".
4.  Copy the "WDAGUtilityAccount" directory to "C:\Users".
(for information, to be sure of copying all files and avoid copy permissions: change the ownership of these various directories and, once the copies have been made,
 restore the original ownership (directory ownership->security tab->advanced ownership).  
  To be done all the time when you launch the modfied VM (you can configure it in the VM launch file in the <LogonCommand>fix_env_vars_import_registry.cmd</LogonCommand > part).

## !! At this step you can backup the BaseLayer.vhdx file and it will be your new pre-configured VM.

## E- Registry import
1. Import the registry you've exported from the VM by launching regedit from the windows search bar and doing file immporter <your *.reg> file.

## F- Set send variables
1. there's a problem when creating C#, for example, and the Microsoft.NET.Sdk is not found, so you need to add "SYSTEM" environment variables. Here's the list of variables with the "fix_dotnet_directory_system_path.bat" script
```
@echo off	
set variables="C:\Program Files (x86)\dotnet;C:\Program Files\dotnet;C:\Program Files (x86)\Windows Kits\10\Windows Performance Toolkit;C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn;C:\Program Files\Microsoft SQL Server\130\Tools\Binn"
for %%p in (%variables%) do (
	set "new_path=%%p"
	set "current_path=%PATH%"
	echo %current_path% | findstr /i /c:"%new_path%" >nul
	if %errorlevel%==0 (
		echo Le chemin %new_path% est déjà dans PATH.
	) else (
		setx PATH "%current_path%;%new_path%" /M
		echo Le chemin %new_path% a été ajouté à PATH.
		set "PATH=%PATH%;%new_path%"
	)
)
```

## Alternatively METHOD , map the host directories to the VM from the setupVisualStudio2022Community.wsb launch file, starting from the BaseLayer.vhdx image whose size you've modified above.
1. You'll need the WDAGUtilityAccount directory, and you'll need to copy it to "C:\Users" (see above for copying instructions and copy problems).
2. You'll need the exported registry file (see above)
3. You'll need to set the environment variables (see above for details) and c++ project you have to fix missing dll (I didn't try to fix it that way, though.)
4. You'll need to configure the "setupVisualStudio2022Community.wsb" file as follows

```
<Configuration>
 <Networking>Enable</Networking>
	<ProtectedClient>Enable</ProtectedClient>
	<PrinterRedirection>Disable</PrinterRedirection>
	<ClipboardRedirection>Disable</ClipboardRedirection>
	<vGpu>Disable</vGpu>
	<MemoryInMB>10240</MemoryInMB>
  <MappedFolders>
	<MappedFolder>
	  <HostFolder>PATH_TO_YOUR_DIRECTORY_WITH_DIRECTORIES_REG_FILE_TO_IMPORT</HostFolder>
	  <SandboxFolder>C:\Users\WDAGUtilityAccount\Downloads\Package-Deploy</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<!--START C:\Program Files-->
	
	<MappedFolder>
	  <HostFolder>C:\Program Files\dotnet</HostFolder>
	  <SandboxFolder>C:\Program Files\dotnet</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>

	<MappedFolder>
	  <HostFolder>C:\Program Files\Common Files\microsoft shared</HostFolder>
	  <SandboxFolder>C:\Program Files\Common Files\microsoft shared</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	
	<MappedFolder>
	  <HostFolder>C:\Program Files\Common Files\Microsoft</HostFolder>
	  <SandboxFolder>C:\Program Files\Common Files\Microsoft</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	
	<MappedFolder>
	  <HostFolder>C:\Program Files\IIS</HostFolder>
	  <SandboxFolder>C:\Program Files\IIS</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<MappedFolder>
	  <HostFolder>C:\Program Files\IIS Express</HostFolder>
	  <SandboxFolder>C:\Program Files\IIS Express</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	
	<MappedFolder>
	  <HostFolder>C:\Program Files\Microsoft SQL Server</HostFolder>
	  <SandboxFolder>C:\Program Files\Microsoft SQL Server</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<MappedFolder>
	  <HostFolder>C:\Program Files\Microsoft Visual Studio</HostFolder>
	  <SandboxFolder>C:\Program Files\Microsoft Visual Studio</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<MappedFolder>
	  <HostFolder>C:\Program Files\MSBuild</HostFolder>
	  <SandboxFolder>C:\Program Files\MSBuild</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<!--END C:\Program Files-->
	
	<!--START C:\Program Files (x86)-->
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\dotnet</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\dotnet</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>

	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\Common Files\Microsoft Shared</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\Common Files\Microsoft Shared</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\Common Files\Microsoft</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\Common Files\Microsoft</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\IIS</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\IIS</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\IIS Express</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\IIS Express</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
		
	<MappedFolder>
		<HostFolder>C:\Program Files (x86)\Microsoft\Xamarin</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\Microsoft\Xamarin</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>

	
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\Microsoft SDKs</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\Microsoft SDKs</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\Microsoft SQL Server</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\Microsoft SQL Server</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\Microsoft Visual Studio</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\Microsoft Visual Studio</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\Microsoft Web Tools</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\Microsoft Web Tools</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\Microsoft.NET</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\Microsoft.NET</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\MSBuild</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\MSBuild</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\NuGet</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\NuGet</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<MappedFolder>
	  <HostFolder>C:\Program Files (x86)\Windows Kits</HostFolder>
	  <SandboxFolder>C:\Program Files (x86)\Windows Kits</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<!--END C:\Program Files (x86)-->
	
	<!--START C:\ProgramData-->
	<MappedFolder>
	  <HostFolder>C:\ProgramData\Microsoft Visual Studio</HostFolder>
	  <SandboxFolder>C:\ProgramData\Microsoft Visual Studio</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	
	<MappedFolder>
	  <HostFolder>C:\ProgramData\Microsoft\VisualStudio</HostFolder>
	  <SandboxFolder>C:\ProgramData\Microsoft\VisualStudio</SandboxFolder>
	  <ReadOnly>true</ReadOnly>
	</MappedFolder>
	
	<!--END C:\ProgramData-->
	
  </MappedFolders> 
</Configuration>
```  
   
   
