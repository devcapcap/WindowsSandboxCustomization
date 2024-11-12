Set UAC = CreateObject("Shell.Application") 
UAC.ShellExecute "cmd.exe", "/c C:\Users\WDAGUtilityAccount\Downloads\Package-Deploy\DumpOfVisualStudio2022CommunityPreInstalled\setup_visual_studio_env_vars.bat", "", "runas", 1
