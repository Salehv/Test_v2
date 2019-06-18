import threading
import sys
import nooboedit as db
import time
import colorama

colorama.init()


def show_help():
    print(
        """Usage: noobodb [EDIT_TYPE] [MODE] [VALUE]
EDIT_TYPE:
	add			to add a list of words to database.
	remove		to remove a list of words.
	rebuild		to rebuild guids of a list of words

MODE:
	-f			read from a file
	-cs			comma separated list
	
VALUE:
	filename or comma separated list of ids
			
		""")


class SqlThread(threading.Thread):

    def __init__(self, rows, s, e, conn):
        threading.Thread.__init__(self)
        self.id = id
        self.rows = rows
        self.msg = colorama.Fore.GREEN + "----------   Range(" + str(s) + "," + str(e) + ") Finished ----------" + \
                   colorama.Style.RESET_ALL
        self.conn = conn

    def run(self):
        for row in self.rows:
            db.GenerateGuide(word=row[1], id=row[0], conn=self.conn, debug=False)
        print(self.msg)


if __name__ == "__main__":
    if (len(sys.argv) < 3):
        show_help()
    else:
        conn = db.get_connection()

        edit_type = sys.argv[1]
        mode = sys.argv[2]
        value = ""
        option = ""
        try:
            value = sys.argv[3]
            option = sys.argv[4]
        except:
            pass
        total_time = time.time()

        if (edit_type == "add"):
            if (mode == "-cs"):
                print("-cs mode not supported for add")
            elif (mode == "-f"):
                with open(value, encoding="utf-8") as file:
                    words = file.read().split("\n")
                    for word in words:
                        id = db.AddWordToDatabase(word=word, conn=conn)
                        db.GenerateGuide(word=word, id=id, conn=conn, branch=True, debug=True)
            else:
                print("Unrecognized mode \"" + mode + "\"")


        elif (edit_type == "remove"):
            if (mode == "-cs"):
                print("NOT IMPELEMENTED YET!!")
            elif (mode == "-f"):
                with open(value, encoding="utf-8") as file:
                    ws = file.read().split("\n")
                    for w in ws:
                        row = db.GetRowByWord(w, conn)

                        permission = input("Do you want to remove '" + row[1] + "' from database? (y/n)")
                        if (permission == "n"):
                            continue

                        db.RemoveWord(row, conn)


            else:
                print("Unrecognized mode \"" + mode + "\"")


        elif (edit_type == "rebuild"):
            if (mode == "-cs"):
                ids = value.split(",")
                for id in ids:
                    row = db.GetRowByID(id, conn)
                    if (option == "-b"):
                        db.GenerateGuide(word=row[1], id=row[0], conn=conn, branch=True)
                    else:
                        db.GenerateGuide(word=row[1], id=row[0], conn=conn, debug=True)

            elif (mode == "-f"):
                with open(value, encoding="utf-8") as file:
                    ids = file.read().split("\n")
                    for id in ids:
                        row = db.GetRowByID(id, conn)
                        if (option == "-b"):
                            db.GenerateGuide(word=row[1], id=row[0], conn=conn, branch=True)
                        else:
                            db.GenerateGuide(word=row[1], id=row[0], conn=conn, debug=True)
            elif (mode == "-all"):
                rows = db.GetAll(conn)
                if (value == "par"):
                    g = option.split("-")
                    ts = []
                    for i in range(int(g[0]) // int(g[2]), int(g[1]) // int(g[2])):
                        ts.append(
                            SqlThread(rows[i * int(g[2]): (i + 1) * int(g[2])], i * int(g[2]), (i + 1) * int(g[2]),
                                      db.get_connection()))
                        ts[-1].start()

                    for t in ts:
                        t.join()

                else:

                    for row in rows[:10]:
                        db.GenerateGuide(word=row[1], id=row[0], conn=conn, debug=False)
                print(colorama.Fore.RED + "[Total Time >> " + str(int((time.time() - total_time) // 60)) + ":" + str(
                    int(time.time() - total_time) % 60) + "]" + colorama.Style.RESET_ALL)


            else:
                print("Unrecognized mode \"" + mode + "\"")

        elif (edit_type == "check"):
            if (mode == "-f"):
                with open(value, encoding="utf-8") as file:
                    ws = file.read().split("\n")

                    if(len(ws) % 2 != 0):
                        print("Wrong Input")
                        quit()

                    with open("output.txt", "w+") as f:
                        pass

                    for i in range(0, len(ws), 2):
                        try:
                            print("From " + ws[i] + " To " + ws[i + 1] + ":")

                            startPoint = db.GetRowByWord(ws[i], conn)[0]
                            endPoint = db.GetRowByWord(ws[i + 1], conn)[0]

                            route = db.FindRoute(int(startPoint), int(endPoint), conn)

                            with open("output.txt", "a") as f:
                                for id in route:
                                    f.write(db.GetRowByID(id, conn)[1] + "\n")
                                f.write("--------------\n")
                        except IndexError as e:
                            print(colorama.Fore.RED + "YOUR START OR END POINT IS NOT IN DATABASE!" + colorama.Style.RESET_ALL)
                        except TypeError as e:
                            pass

        elif (edit_type == "related"):
            if (mode == "-f"):
                with open(value, encoding="utf-8") as file:
                    w = file.read().split("\n")[0]

                    startPoint = db.GetRowByWord(w, conn)
                    with open("output.txt", "w+") as f:
                        for rw in db.GetRelatedWords(startPoint[0], conn):
                            f.write(rw + "\n")

        elif (edit_type == "help"):
            show_help()


        else:
            print("Unrecognized Command!")
