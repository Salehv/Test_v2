import mysql.connector

mydb = mysql.connector.connect(
  host="localhost",
  user="root",
  passwd="",
  database="kalanjar"
)
c = mydb.cursor()

def get_similar_diff_char(word, similar):
	if(len(similar) == len(word)):
		for i in range(len(word)):
			if(word[i] != similar[i]):
				return similar[i]	
	elif(len(similar) > len(word)):
		for i in range(len(similar)):
			if(i == len(word)):
				return similar[i]
				break 
			elif(similar[i] != word[i]):
				return similar[i]
				break 
	return ""

def Add(word):
	#try:
	word
	
	# Added
	c.execute("SELECT * FROM `words` WHERE levenshtein('" + word +"', `word`) = 1;")
	similars = c.fetchall()
	
	ld = ""
	for similar in similars:
		ld = ld + str(similar[0]) + ","
	
	chars = ""
	for s in similars:
		similar = s[1]
		if(similar != word):
			ch = get_similar_diff_char(word, similar)
			if ch == "": continue
			if ch not in chars: chars += ch + ","
			
			'''
			f(len(similar) == len(word)):
				for i in range(len(word)):
					if(word[i] != similar[i]):
						if(similar[i] not in chars):
							chars += similar[i] + ","
			elif(len(similar) > len(word)):
				for i in range(len(similar)):
					if(i == len(word)):
						if(similar[i] not in chars):
							chars += similar[i] + ","
						break 
					elif(similar[i] != word[i]):
						if(similar[i] not in chars):	
							chars += similar[i] + ","
						break 
			'''
	
	sql = "INSERT INTO `words` VALUES (%s, %s, %s, %s);"
	val = (0, word, chars[:-1], ld[:-1])
	c.execute(sql, val)
	mydb.commit()
	
	'''
	sql = "UPDATE `words` \
		SET `relate`='" + chars[:-1] + "', `ld`='" + ld[:-1] + "' \
		WHERE `id`=" + str(row[0])
	
	c.execute(sql)
	print(str(row[0]) + " Done! [Time Taken: " + str(time.time() - t) + "]")
	total_time += time.time() - t
	print("----------------------  Total Time:" + str(total_time))
	'''
	
with open("toAdd.txt") as f:
	for line in f.split("\n"):
		Add(line)
