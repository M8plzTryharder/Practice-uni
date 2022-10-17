import hashlib
from math import ceil
import time
import multiprocessing
import threading

alphabet = "abcdefghijklmnopqrstuvwxyz"

def gen_pass(index):
    password = ""
    for i in range(5):
        password += alphabet[index % len(alphabet)]
        index = index // len(alphabet)
    return password[::-1]

def sha256_selection(index, hashes, start, end):
    for i in range(start, end):
        password = gen_pass(i)
        hash = hashlib.sha256(password.encode()).hexdigest()
        for h in hashes:
            if hash != h:
                continue
            print("Хэш {} найден пароль: {}. Поток {}.".format(hash, password, index))

def sha256():
    start_time = time.perf_counter()
    hashes = ["1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad",
              "3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b",
              "74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f"]
    P = 26**5 #26 - мощность алфавита, 5 - знаков в искомом пароле
    n_threads = int(input("Количество потоков для SHA256:"))
    if n_threads < 1:
        print("Введите количество потоков: ")
        n_threads = int(input)
    per_thread =ceil(P / n_threads)

    threads = []
    start = 0
    for i in range(n_threads):
        end = start + per_thread
        t = threading.Thread(target=sha256_selection, args=(i, hashes, start, end))
        threads.append(t)
        t.start()
        start = end
    for thread in threads:
        thread.join()
    end_time = time.perf_counter()
    elapsed_time = end_time - start_time
    print("Время выполнения: {}.".format(elapsed_time))

def md5_selection(index, hashes, start, end):
    for i in range(start, end):
        password = gen_pass(i)
        hash = hashlib.md5(password.encode()).hexdigest()
        for h in hashes:
            if hash != h:
                continue
            print("Хэш {} найден пароль: {}. Поток {}.".format(hash, password, index))

def md5():
    start_time = time.perf_counter()

    hashes = ["81d45c9cf678fbaa8d64a6f29a6f97e3",
              "1f3870be274f6c49b3e31a0c6728957f",
              "d9308f32f8c6cf370ca5aaaeafc0d49b"]
    P = 26**5
    n_threads = int(input("Количество потоков для MD5:"))
    if n_threads < 1:
        print("Введите количество потоков: ")
        n_threads = int(input)
    per_thread =ceil(P / n_threads)

    threads = []
    start = 0
    for i in range(n_threads):
        end = start + per_thread
        proc = multiprocessing.Process(target=md5_selection, args=(i, hashes, start, end))
        threads.append(proc)
        proc.start()
        start = end
    for thread in threads:
        thread.join()
    end_time = time.perf_counter()
    elapsed_time = end_time - start_time
    print("Время выполнения: {}.".format(elapsed_time))

if __name__ == '__main__':
    n_proc = multiprocessing.cpu_count()
    print(f"Кол-во ядер на данном компьютере: {n_proc}")
    sha256()
    md5()
