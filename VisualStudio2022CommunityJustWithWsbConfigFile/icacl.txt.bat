for /l %%x in (1, 1, 1000) do (
echo %%x
takeown /r /d y /f *
)