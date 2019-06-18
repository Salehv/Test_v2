all2 = []
with open("Text.txt", encoding="utf-8") as f:
	txt = f.read()
	all = txt.split("\n")
	for i in range(len(all) - 1):
		print(i)
		all2.append(all[i])
		for j in range(i + 1, len(all) - 1):
			if(all[j] == all[i]):
				del(all2[-1])
				break

with open("Text2.txt", "w", encoding="utf-8") as f:
	for i in all2:
		f.write(i + "\n")
	f.flush()
