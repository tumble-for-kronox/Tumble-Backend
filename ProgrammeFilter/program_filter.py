import os
import sys
from typing import List
import re
import requests
import scrapy
from pymongo import MongoClient
from pymongo.errors import ConnectionFailure, OperationFailure
from dotenv import load_dotenv
from datetime import datetime
import certifi

SCHOOL_DICT = {
    "hkr": "schema.hkr.se",
    "mau": "schema.mau.se",
    "oru": "schema.oru.se",
    # ! "ltu": "schema.ltu.se" requires login,
    "hig": "schema.hig.se",
    # ! "sh": "kronox.sh.se" requires login,
    "hv": "schema.hv.se",
    "hb": "schema.hb.se",
    # ! "mdh": "schema.mdh.se" requires login,
}

SCHOOL_YEAR_REGEX_DICT = {
    "hkr": r"\d{2}(\d{2})",
    "mau": r"^\D*(\d{2})",
    "hig": r"\d{2}$",
    "hv": r"(\d{2})-$",
    "hb": r"^\D*\d*(?=\d{2})(\d{2})",
}

YEAR_REGEX_GROUP_DICT = {
    "hkr": 1,
    "mau": 1,
    "hig": 0,
    "hv": 1,
    "hb": 1,
}

if os.path.exists(os.path.join(os.path.dirname(__file__), ".env")):
    load_dotenv(os.path.join(os.path.dirname(__file__), ".env"))

ca = certifi.where()
MONGO_CLIENT = MongoClient(os.environ.get("DbConnectionString"), tlsCAFile=ca)

try:
    MONGO_CLIENT.admin.command("ping")
except ConnectionFailure as err:
    print(f"Connection to DB failed. Error: {err}")
    sys.exit()
except OperationFailure as err:
    print(f"Connection to DB failed. Error: {err}")
    sys.exit()


FILTER_DB = MONGO_CLIENT["test_db"].get_collection("programme_filters")


def main():
    for (schoolId, schoolUrl) in SCHOOL_DICT.items():
        updateFilterForSchool(schoolId, schoolUrl)


def updateFilterForSchool(schoolId: str, schoolUrl: str):
    """
    docstring
    """
    allScheduleIds = getAllIds(schoolUrl)

    inactiveIds = []

    for (i, id) in enumerate(allScheduleIds):
        if scheduleInactive(schoolId, schoolUrl, id):
            inactiveIds.append(id)
        if i % 10 == 0:
            print(f"\r{((i / len(allScheduleIds)) * 100):.2f}% of {schoolId} completed!", end="")

    saveSchoolFilterList(schoolId, inactiveIds)


def saveSchoolFilterList(schoolId: str, inactiveIds: List[str]):
    """
    docstring
    """
    FILTER_DB.update_one(
        {"_id": schoolId},
        {
            "$set": {
                "filter": inactiveIds,
            }
        },
        upsert=True,
    )
    print("\033[K")
    print(f"\rSaved filter for {schoolId}")


def getAllIds(schoolUrl: str) -> List[str]:
    """
    docstring
    """
    resp = requests.get(
        f"https://{schoolUrl}/ajax/ajax_resurser.jsp?op=hamtaResursDialog&resurstyp=UTB_PROGRAMINSTANS_KLASSER"
    )

    selector = scrapy.Selector(text=resp.text)

    return ["p." + x.replace(" ", "+") for x in selector.xpath("//tr/td[2]/text()").getall()]


def scheduleInactive(schoolId: str, schoolUrl: str, scheduleId: str) -> bool:
    """
    docstring
    """
    try:
        if (
            schoolId != "oru"
            and re.search(SCHOOL_YEAR_REGEX_DICT[schoolId], scheduleId)
            and int(str(datetime.now().year)[2:])
            - int(re.search(SCHOOL_YEAR_REGEX_DICT[schoolId], scheduleId).group(YEAR_REGEX_GROUP_DICT[schoolId]))
            > 5
        ):
            return True

        if (
            schoolId != "oru"
            and re.search(SCHOOL_YEAR_REGEX_DICT[schoolId], scheduleId)
            and int(str(datetime.now().year)[2:])
            - int(re.search(SCHOOL_YEAR_REGEX_DICT[schoolId], scheduleId).group(YEAR_REGEX_GROUP_DICT[schoolId]))
            < 0
        ):
            return False
    except ValueError:
        pass

    resp = requests.get(
        f"https://{schoolUrl}/setup/jsp/SchemaXML.jsp?startDatum=idag&intervallTyp=m&intervallAntal=6&sprak=SV&sokMedAND=true&forklaringar=true&resurser={scheduleId}"  # noqa: E501
    )

    return len(resp.content) <= 1565


if __name__ == "__main__":
    main()
