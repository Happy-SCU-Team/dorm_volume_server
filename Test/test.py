import requests
import json
west_dorm="west_dorm"
test_segment={"account":west_dorm,"segments":[{"start_time":0,"end_time":12},{"start_time":34,"end_time":235}]}
test_new_name={"account":west_dorm,"new_account":"123"}

def get_segment():
    a=requests.get("http://localhost:5000/123/schedule")
    print(a.text)

def post_segment():
    b=requests.post("http://localhost:5000/update/schedule",json=test_segment)
    print(b.text)
def update_account_name():
    b=requests.post("http://localhost:5000/update/account",json=test_new_name)
    print(b.text)

post_segment()
update_account_name()
get_segment()