import mysql.connector

mydb = mysql.connector.connect(
  host="localhost",
  user="root",
  passwd="",
  database="kalanjar"
)

c = mydb.cursor()
sql = "INSERT INTO `words` VALUES (%s, %s, %s, %s)"
val = [];

with open("Final.txt", encoding="utf-8") as f:
	txt = f.read()
	for l in txt.split("\n"):
		p = (0, l, None, None)
		val.append(p);

# print(val)
c.executemany(sql, val)

mydb.commit()

print(c)
