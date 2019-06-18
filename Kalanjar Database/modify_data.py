import mysql.connector


def Add(word, c):
	
	sql = "INSERT INTO `words` VALUES (%s, %s, %s, %s);"
	val = (0, word, "", "")
	c.execute(sql, val)
	
	
if __name__ == "__main__":
	import generate_guides
	
	while True:
		newFile = ""
		with open("toAdd.txt", encoding="utf-8") as f:
			lines = f.read().split("\n");
			line = lines[0]
			
			if line == '':
				quit()
			
			mydb = mysql.connector.connect(
			  host="localhost",
			  user="root",
			  passwd="",
			  database="kalanjar"
			)
			c = mydb.cursor()
			try:
				Add(line, c)
			except mysql.connector.errors.IntegrityError:
				pass
			else:
				mydb.commit()
				mydb.close()
				generate_guides.generate([[c.lastrowid, line]], True)
			if len(lines) <= 1:
				break
			newFile = "\n".join(lines[1:])
			
		with open("toAdd.txt", "w", encoding="utf-8") as f:
			f.write(newFile)
			f.flush()