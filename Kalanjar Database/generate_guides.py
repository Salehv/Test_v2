import threading
import time
import mysql.connector

mydb = mysql.connector.connect(
  host="localhost",
  user="root",
  passwd="",
  database="kalanjar"
)
c = mydb.cursor()

'''
class myThread (threading.Thread):
   def __init__(self, threadID, items):
      threading.Thread.__init__(self)
      self.threadID = threadID
      self.items = items
   def run(self):
      print ("Starting " + str(self.threadID))
      f(self.items)
'''
total_time = 0
def generate(rows, first):
	global total_time
	for row in rows:
		print("Row: " + str(row))
		t = time.time()
		sql = "SELECT * FROM `words` WHERE levenshtein('" + row[1] +"', `word`) = 1;"
		#print("SQL: " + sql)
		c.execute(sql)
		
		similars = c.fetchall()
		print(similars)
		
		ld = []
		ldw = []
		for similar in similars:
			ld.append(str(similar[0]))
			ldw.append(similar[1])
		print("ld: " + str(ld))
		
		chars = ""
		for s in similars:
			similar = s[1]
			if(similar != row[1]):
				if(len(similar) == len(row[1])):
					for i in range(len(row[1])):
						if(row[1][i] != similar[i]):
							if(similar[i] not in chars):
								chars += similar[i] + ","
				elif(len(similar) > len(row[1])):
					for i in range(len(similar)):
						if(i == len(row[1])):
							if(similar[i] not in chars):
								chars += similar[i] + ","
							break 
						elif(similar[i] != row[1][i]):
							if(similar[i] not in chars):	
								chars += similar[i] + ","
							break 
		print("Chars: " + chars)
		sql = "UPDATE `words` SET `relate`='" + chars[:-1] + "', `ld`='" + ",".join(ld) + "' WHERE `id`=" + str(row[0])
		c.execute(sql)
		#print(sql)
		
		print(str(row[0]) + " Done! [Time Taken: " + str(time.time() - t) + "]")
		total_time += time.time() - t
		print("----------------------  Total Time:" + str(total_time))
		if __name__ != "__main__":
			if first:
				for w in range(len(ldw)):
					generate([[ld[w],ldw[w]]], False)
			mydb.commit()


if __name__ == "__main__":
	c.execute("SELECT * FROM `words`;")

	rows = c.fetchall()
	generate(rows, True)

	mydb.commit()
	mydb.close()

