def GetPage(path):
	return route[path]()
	
def IndexPage():
	with open("html/index.html", "rb") as f:
		return f.read()

def AddWordPage():
	with open("html/add.html", "rb") as f:
		return f.read()
		
def ParsePostRequset(req):
	items = {}
	pairs = req.split("&")
	for pair in pairs:
		key, value = pair.split("=")
		items[key] = value
	return items	

route = {"/":IndexPage, "/Add":AddWordPage, "/Fail":IndexPage, "Success":IndexPage}