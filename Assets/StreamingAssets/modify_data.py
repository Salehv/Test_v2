import sqlite3


def create_connection(db_file):
    """ create a database connection to the SQLite database
        specified by db_file
    :param db_file: database file
    :return: Connection object or None
    """
    try:
        conn = sqlite3.connect(db_file)
        return conn
    except Error as e:
        print(e)
 
    return None


	
conn = create_connection("progress.db3")

sql = "INSERT INTO progress VALUES(?,?,?,?,?)"

chaps = []

for i in range(16):
    chaps.append((i, -1, 0, "", 1))

for j in range(1, 6):
    for i in range(28):
        chaps.append((j * 100 + i, -1, 0, "", 0))

print(chaps)
with conn:
    cur = conn.cursor()
    cur.executemany(sql, chaps)
    