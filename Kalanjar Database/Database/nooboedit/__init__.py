import time
import mysql.connector
import heapq
import colorama


def get_connection():
    conn = mysql.connector.connect(
        host="localhost",
        user="root",
        passwd="",
        database="kalanjar"
    )
    return conn


def GetRowByWord(word, conn):
    cursor = conn.cursor()
    sql = "SELECT * FROM `words` WHERE `word`='" + word + "';"
    cursor.execute(sql)
    try:
        return cursor.fetchall()[0]
    except IndexError:
        _error("Word Not Found => \"" + word + "\"")
        quit()


def GetRowByID(id, conn):
    cursor = conn.cursor()
    sql = "SELECT * FROM `words` WHERE `id`=" + str(id) + ";"
    cursor.execute(sql)
    fetched = []
    try:
        fetched = cursor.fetchall()[0]
    except:
        print("Cannot Find ID:" + str(id))
        quit()

    return fetched


def GenerateGuide(word, id, conn, branch=False, debug=False):
    cursor = conn.cursor()

    print("Record: [" + str(id) + ", " + word + "]")

    t = time.time()
    sql = "SELECT * FROM `words` WHERE levenshtein('" + word + "', `word`) = 1;"

    if (debug):
        print("[DEBUG] Select SQL: " + sql)

    cursor.execute(sql)
    similars = cursor.fetchall()

    if (debug):
        print("[DEBUG] Similars:" + "\n".join(list(map(lambda x: x[1], similars))))

    ld = []
    ldw = []
    for similar in similars:
        ld.append(str(similar[0]))
        ldw.append(similar[1])

    if (debug):
        print("[DEBUG] Related IDs: " + str(ld))

    chars = ""
    for s in ldw:
        c = _get_differ_char(word, s)
        if c == "" or c in chars:
            pass
        else:
            chars += c + ","

    if (debug):
        print("[DEBUG] Chars: " + chars)

    sql = "UPDATE `words` SET `relate`='" + chars[:-1] + "', `ld`='" + ",".join(ld) + "' WHERE `id`=" + str(id)
    cursor.execute(sql)

    if (debug):
        print("[DEBUG] related and chars: " + sql)

    conn.commit()

    print(str(id) + " Done! [Time Taken: " + str(time.time() - t) + "]")

    if branch:
        for w in range(len(ldw)):
            GenerateGuide(word=ldw[w], id=ld[w], conn=conn)

    return t


def AddWordToDatabase(word, conn):
    cursor = conn.cursor()

    sql = "INSERT INTO `words` VALUES (%s, %s, %s, %s)"
    val = (0, word, "", "");

    cursor.execute(sql, val)

    conn.commit()

    return cursor.lastrowid


def RemoveWord(row, conn):
    cursor = conn.cursor()
    sql = "DELETE FROM `words` WHERE `id`=" + str(row[0])
    cursor.execute(sql)
    conn.commit()

    lds = row[3].split(",")
    for ld in lds:
        ldrow = GetRowByID(ld, conn)
        GenerateGuide(word=ldrow[1], id=ldrow[0], conn=conn)


def FindRoute(startPoint, endPoint, conn):
    max_val = float('Inf')

    distance = _dijkstra(str(startPoint), max_val, conn)
    '''for n in distance:
        print(str(n) + ":" + str(distance[n]))
'''
    try:
        curr = distance[str(endPoint)]
        visits = [str(endPoint)]
        while curr[0] != 0:
            if curr[0] == max_val:
                print(-1)
                return
            visits.append(curr[1])
            curr = distance[curr[1]]
        print(visits)
        return reversed(visits)
    except KeyError as e:
        print(colorama.Fore.RED + "NOT REACHABLE!" + colorama.Style.RESET_ALL)
        return


def GetAdjacentID(id, conn):
    ids = GetRowByID(id, conn)[3]
    return ids.split(",")


def GetAll(conn):
    cursor = conn.cursor()
    sql = "SELECT * FROM `words`;"
    cursor.execute(sql)
    return cursor.fetchall()


def GetRelatedWords(id, conn):
    relates = []
    ids = GetAdjacentID(id, conn)
    for i in ids:
        relates.append(GetRowByID(int(i), conn)[1])
    return relates


''' Private Methods '''


def _dijkstra(start, max_val, conn):
    distance = dict()
    visited = dict()
    queue = [(0, start, -1)]

    while len(queue) > 0:
        d, n, f = heapq.heappop(queue)
        if n not in visited.keys():
            visited[n] = True
            distance[n] = (d, f)
            for m in GetAdjacentID(str(n), conn):
                if m not in visited.keys():
                    heapq.heappush(queue, (d + 1, m, n))
    return distance


def _get_differ_char(word, similar):
    if (word != similar):
        if (len(similar) == len(word)):
            for i in range(len(word)):
                if (word[i] != similar[i]):
                    return similar[i]
        elif (len(similar) > len(word)):
            for i in range(len(similar)):
                if (i == len(word)):
                    return similar[i]
                elif (similar[i] != word[i]):
                    return similar[i]
    return ""


def _error(err):
    print(colorama.Fore.RED + err + colorama.Style.RESET_ALL)
