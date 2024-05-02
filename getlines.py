import os


lines = 0
list_ = os.listdir(os.curdir)

for i in list_:
    if os.path.isfile(i):
        continue
    files=os.listdir(i)
    for j in files:
        if j.endswith(".cs"):
            f=open(i+"/"+j)
            lines += len(f.readlines())

print(lines)
input("Press Enter to Continue...")
