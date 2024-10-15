// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <iostream>
#include <string>
#include <sstream>
#include <nlohmann/json.hpp>
#include <vector>

// Surgery class
class Surgery {
public:
    std::string SurgeryName;
    int Duration;
    std::string DayOfWeek;
    std::string StartTime;

    Surgery(const std::string& name, int duration, const std::string& day, const std::string& time)
        : SurgeryName(name), Duration(duration), DayOfWeek(day), StartTime(time) {}
};

// Surgeon class
class Surgeon {
public:
    std::string SurgeonName;
    std::vector<Surgery> Surgeries;

    Surgeon(const std::string& name) : SurgeonName(name) {}

    void AddSurgery(const Surgery& surgery) {
        Surgeries.push_back(surgery);
    }
};

// Availability class
class Availability {
public:
    std::string DayOfWeek;
    std::string StartTime;
    std::string EndTime;

    Availability(const std::string& day, const std::string& start, const std::string& end)
        : DayOfWeek(day), StartTime(start), EndTime(end) {}
};

// Anesthesiologist class
class Anesthesiologist {
public:
    std::string AnesthesiologistName;
    std::vector<Availability> AvailableTimes;

    Anesthesiologist(const std::string& name) : AnesthesiologistName(name) {}

    void AddAvailability(const Availability& availability) {
        AvailableTimes.push_back(availability);
    }
};

// HospitalData class
class HospitalData {
public:
    std::vector<Surgeon> Surgeons;
    std::vector<Anesthesiologist> Anesthesiologists;
    int NumberOfOrRooms;

    std::vector<std::vector<Surgery>> ORRooms;

    HospitalData(int orRooms) : NumberOfOrRooms(orRooms) {
        ORRooms.resize(orRooms);
    }

    void AddSurgeon(const Surgeon& surgeon) {
        Surgeons.push_back(surgeon);
    }

    void AddAnesthesiologist(const Anesthesiologist& anesthesiologist) {
        Anesthesiologists.push_back(anesthesiologist);
    }
};


void OrganizeSurgeries(std::vector<Surgeon>& surgeons);
void OrganizeAvailability(std::vector<Anesthesiologist>& anesthesiologists);
std::string CreateORSchedule(HospitalData& hospitalData);
bool AssignSurgeryToRoom(std::vector<std::vector<Surgery>>& rooms, const Surgery& surgery, const std::vector<Anesthesiologist>& anesthesiologists);
int GetDayOfWeekValue(const std::string& day);
int GetTimeInMinutes(const std::string& timeStr);
bool IsConflict(const Surgery& existingSurgery, const Surgery& newSurgery);
bool IsAnesthesiologistAvailable(const Anesthesiologist& anesthesiologist, const Surgery& surgery);

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}


using json = nlohmann::json;

// Comparator function to sort surgeries by DayOfWeek and StartTime
bool SurgeryComparator(const Surgery& s1, const Surgery& s2) {
    int day1 = GetDayOfWeekValue(s1.DayOfWeek);
    int day2 = GetDayOfWeekValue(s2.DayOfWeek);

    if (day1 == day2) {
        int time1 = GetTimeInMinutes(s1.StartTime);
        int time2 = GetTimeInMinutes(s2.StartTime);
        return time1 < time2;
    }
    return day1 < day2;
}

//Turn the string input for day of the week into an integer
int GetDayOfWeekValue(const std::string& day) {
    if (day == "Monday") return 1;
    if (day == "Tuesday") return 2;
    if (day == "Wednesday") return 3;
    if (day == "Thursday") return 4;
    if (day == "Friday") return 5;
    return 0; // Default, or throw an error if invalid day
}

//Turn the string time into integers
int GetTimeInMinutes(const std::string& timeStr) {
    std::istringstream ss(timeStr);
    int hours, minutes;
    char colon, ampm;
    std::string period;

    ss >> hours >> colon >> minutes >> period; // Extract time and period (AM/PM)

    if (period == "PM" && hours != 12) hours += 12;
    if (period == "AM" && hours == 12) hours = 0; // Handle 12 AM case

    return hours * 60 + minutes;
}

// Organize the surgeries for each surgeon by their preferred day and time
void OrganizeSurgeries(std::vector<Surgeon>& surgeons) {
    for (auto& surgeon : surgeons) {
        std::sort(surgeon.Surgeries.begin(), surgeon.Surgeries.end(), SurgeryComparator);
    }
}

// Comparator function to sort availability by DayOfWeek and StartTime
bool AvailabilityComparator(const Availability& a1, const Availability& a2) {
    int day1 = GetDayOfWeekValue(a1.DayOfWeek);
    int day2 = GetDayOfWeekValue(a2.DayOfWeek);

    if (day1 == day2) {
        int time1 = GetTimeInMinutes(a1.StartTime);
        int time2 = GetTimeInMinutes(a2.StartTime);
        return time1 < time2;
    }
    return day1 < day2;
}

// Organize anesthesiologist availability by day and time
void OrganizeAvailability(std::vector<Anesthesiologist>& anesthesiologists) {
    for (auto& anesthesiologist : anesthesiologists) {
        std::sort(anesthesiologist.AvailableTimes.begin(), anesthesiologist.AvailableTimes.end(), AvailabilityComparator);
    }
}

// Check if a surgery conflicts with another surgery
bool IsConflict(const Surgery& existingSurgery, const Surgery& newSurgery) {
    if (existingSurgery.DayOfWeek != newSurgery.DayOfWeek) {
        return false;  // Different days, no conflict
    }

    // Check if the time of surgeries overlap
    int existingStartTime = std::stoi(existingSurgery.StartTime.substr(0, 2)) * 60 + std::stoi(existingSurgery.StartTime.substr(3, 2));
    int existingEndTime = existingStartTime + existingSurgery.Duration;
    int newStartTime = std::stoi(newSurgery.StartTime.substr(0, 2)) * 60 + std::stoi(newSurgery.StartTime.substr(3, 2));
    int newEndTime = newStartTime + newSurgery.Duration;

    return !(newStartTime >= existingEndTime || newEndTime <= existingStartTime);
}

// Check if an anesthesiologist is available during the surgery
bool IsAnesthesiologistAvailable(const Anesthesiologist& anesthesiologist, const Surgery& surgery) {
    for (const auto& availability : anesthesiologist.AvailableTimes) {
        if (availability.DayOfWeek == surgery.DayOfWeek) {
            int startTime = std::stoi(surgery.StartTime.substr(0, 2)) * 60 + std::stoi(surgery.StartTime.substr(3, 2));
            int endTime = startTime + surgery.Duration;

            int availableStartTime = std::stoi(availability.StartTime.substr(0, 2)) * 60 + std::stoi(availability.StartTime.substr(3, 2));
            int availableEndTime = std::stoi(availability.EndTime.substr(0, 2)) * 60 + std::stoi(availability.EndTime.substr(3, 2));

            if (startTime >= availableStartTime && endTime <= availableEndTime) {
                return true;
            }
        }
    }
    return false;
}

// Check for conflicts between surgeries and assign to an OR room if available
bool AssignSurgeryToRoom(std::vector<std::vector<Surgery>>& rooms, const Surgery& surgery, const std::vector<Anesthesiologist>& anesthesiologists) {
    // Loop through each room
    for (auto& room : rooms) {
        bool conflictFound = false;

        // Check for conflicts with surgeries already scheduled in this room
        for (const auto& scheduledSurgery : room) {
            if (IsConflict(scheduledSurgery, surgery)) {
                conflictFound = true;
                break;
            }
        }

        if (!conflictFound) {
            // Check if an anesthesiologist is available for this surgery
            bool anesthesiologistAvailable = false;
            for (const auto& anesthesiologist : anesthesiologists) {
                if (IsAnesthesiologistAvailable(anesthesiologist, surgery)) {
                    anesthesiologistAvailable = true;
                    break;
                }
            }

            if (anesthesiologistAvailable) {
                // No conflict found and anesthesiologist is available, assign surgery to this room
                room.push_back(surgery);
                return true;
            }
        }
    }

    return false; // No available room found for this surgery
}

// Main function to schedule surgeries across OR rooms
std::string CreateORSchedule(HospitalData& hospitalData) {
    // Reference the OR rooms from HospitalData
    std::vector<std::vector<Surgery>>& rooms = hospitalData.ORRooms;

    // Create a vector to store unscheduled surgeries
    std::vector<Surgery> unscheduledSurgeries;

    // Loop through each surgeon's surgeries
    for (const auto& surgeon : hospitalData.Surgeons) {
        for (const auto& surgery : surgeon.Surgeries) {
            // Try to assign the surgery to an available room
            bool assigned = AssignSurgeryToRoom(rooms, surgery, hospitalData.Anesthesiologists);
            if (!assigned) {
                // If not assigned, add it to the unscheduledSurgeries list
                unscheduledSurgeries.push_back(surgery);
            }
        }
    }

    // Build the final schedule in a string stream (if you need to return a string for UI)
    std::stringstream scheduleStream;

    // Add scheduled surgeries to the string
    scheduleStream << "Scheduled Surgeries:\n";
    for (size_t i = 0; i < rooms.size(); ++i) {
        scheduleStream << "OR Room " << i + 1 << " Schedule:\n";
        for (const auto& surgery : rooms[i]) {
            scheduleStream << "  Surgery: " << surgery.SurgeryName << " on " << surgery.DayOfWeek
                << " at " << surgery.StartTime << "\n";
        }
    }

    // Add unscheduled surgeries to the string
    if (!unscheduledSurgeries.empty()) {
        scheduleStream << "\nUnscheduled Surgeries:\n";
        for (const auto& surgery : unscheduledSurgeries) {
            scheduleStream << "  Surgery: " << surgery.SurgeryName << " on " << surgery.DayOfWeek
                << " at " << surgery.StartTime << " (Duration: " << surgery.Duration << ")\n";
        }
    }
    else {
        scheduleStream << "\nAll surgeries were successfully scheduled.\n";
    }

    // Return the schedule as a string
    return scheduleStream.str();
}


// Function to parse surgeons and their surgeries from JSON
void ParseSurgeonsFromJson(const json& hospitalDataJson, HospitalData& hospitalData) {
    for (const auto& surgeonJson : hospitalDataJson["Surgeons"]) {
        std::string surgeonName = surgeonJson["SurgeonName"];
        Surgeon surgeon(surgeonName);

        // Iterate through surgeries
        for (const auto& surgeryJson : surgeonJson["Surgeries"]) {
            std::string surgeryName = surgeryJson["SurgeryName"];
            int duration = surgeryJson["Duration"];
            std::string dayOfWeek = surgeryJson["DayOfWeek"];
            std::string startTime = surgeryJson["StartTime"];

            // Create Surgery object and add to the surgeon
            Surgery surgery(surgeryName, duration, dayOfWeek, startTime);
            surgeon.AddSurgery(surgery);
        }

        // Add surgeon to the hospital data
        hospitalData.AddSurgeon(surgeon);
    }
}

// Function to parse anesthesiologists and their availability from JSON
void ParseAnesthesiologistsFromJson(const json& hospitalDataJson, HospitalData& hospitalData) {
    for (const auto& anesthesiologistJson : hospitalDataJson["Anesthesiologists"]) {
        std::string anesthesiologistName = anesthesiologistJson["AnesthesiologistName"];
        Anesthesiologist anesthesiologist(anesthesiologistName);

        // Iterate through available times
        for (const auto& availableTimeJson : anesthesiologistJson["AvailableTimes"]) {
            std::string dayOfWeek = availableTimeJson["DayOfWeek"];
            std::string startTime = availableTimeJson["StartTime"];
            std::string endTime = availableTimeJson["EndTime"];

            // Create Availability object and add to the anesthesiologist
            Availability availability(dayOfWeek, startTime, endTime);
            anesthesiologist.AddAvailability(availability);
        }

        // Add anesthesiologist to the hospital data
        hospitalData.AddAnesthesiologist(anesthesiologist);
    }
}

// Function to generate schedule JSON from HospitalData

std::string GenerateScheduleJson(const HospitalData& hospitalData) {
    json scheduleJson;

    // Iterate through OR rooms and add surgeries to the JSON
    for (size_t i = 0; i < hospitalData.ORRooms.size(); ++i) {
        json roomJson;

        for (const auto& surgery : hospitalData.ORRooms[i]) {
            // Find the surgeon's name for this surgery
            std::string surgeonName;

            for (const auto& surgeon : hospitalData.Surgeons) {
                // Check if this surgeon performed the surgery by matching surgery names
                for (const auto& surgeonSurgery : surgeon.Surgeries) {
                    if (surgery.SurgeryName == surgeonSurgery.SurgeryName) {
                        surgeonName = surgeon.SurgeonName;  // Get the surgeon's name
                        break;
                    }
                }
                if (!surgeonName.empty()) break;  // If we found the surgeon, break out of the loop
            }

            // Add the surgery information to the room JSON, including the correct surgeon's name
            roomJson.push_back({
                {"SurgeryName", surgery.SurgeryName},
                {"SurgeonName", surgeonName}, 
                {"DayOfWeek", surgery.DayOfWeek},
                {"StartTime", surgery.StartTime},
                {"Duration", surgery.Duration}
                });
        }

        scheduleJson["ORRooms"].push_back(roomJson);
    }

    // Convert the JSON to string and return
    return scheduleJson.dump();
}

#include <iostream>
#include <stdexcept>
#include <cstring>
// Assuming you're using the nlohmann::json library

// Using nlohmann::json
using json = nlohmann::json;

extern "C" __declspec(dllexport) const char* ReceiveHospitalData(const char* jsonData) {
    try {
        std::cout << "ReceiveHospitalData called." << std::endl;

        // Parse the incoming JSON data
        json hospitalDataJson = json::parse(jsonData);

        // Extract number of OR rooms and create HospitalData instance
        if (hospitalDataJson.contains("NumberOfOrRooms") && !hospitalDataJson["NumberOfOrRooms"].is_null()) {
            int numberOfORRooms = hospitalDataJson["NumberOfOrRooms"];
            HospitalData hospitalData(numberOfORRooms);

            // Parse surgeons and anesthesiologists from JSON
            ParseSurgeonsFromJson(hospitalDataJson, hospitalData);
            ParseAnesthesiologistsFromJson(hospitalDataJson, hospitalData);

            // Organize surgeries and availability
            OrganizeSurgeries(hospitalData.Surgeons);
            OrganizeAvailability(hospitalData.Anesthesiologists);

            // Call CreateORSchedule to generate the schedule
            std::string scheduleStr = CreateORSchedule(hospitalData);

            // Convert schedule string to C-style string to return to C#
            char* returnSchedule = new char[scheduleStr.size() + 1];
            strcpy_s(returnSchedule, scheduleStr.size() + 1, scheduleStr.c_str());

            return returnSchedule;
        }
        else {
            std::cerr << "Error: NumberOfOrRooms is missing or null in JSON." << std::endl;
            return nullptr;
        }
    }
    catch (const std::exception& e) {
        std::cerr << "Exception: " << e.what() << std::endl;
        return nullptr;
    }
    catch (...) {
        std::cerr << "Unknown error occurred in ReceiveHospitalData." << std::endl;
        return nullptr;
    }
}



/* Output for debugging
std::cout << "Hospital Data:" << std::endl;
std::cout << "Number of OR Rooms: " << hospitalData.NumberOfOrRooms << std::endl;

for (const auto& surgeon : hospitalData.Surgeons) {
    std::cout << "Surgeon: " << surgeon.SurgeonName << std::endl;
    for (const auto& surgery : surgeon.Surgeries) {
        std::cout << "  Surgery: " << surgery.SurgeryName << ", Duration: " << surgery.Duration
            << ", Day: " << surgery.DayOfWeek << ", Start Time: " << surgery.StartTime << std::endl;
    }
}

for (const auto& anesthesiologist : hospitalData.Anesthesiologists) {
    std::cout << "Anesthesiologist: " << anesthesiologist.AnesthesiologistName << std::endl;
    for (const auto& availability : anesthesiologist.AvailableTimes) {
        std::cout << "  Day: " << availability.DayOfWeek << ", Start Time: " << availability.StartTime
            << ", End Time: " << availability.EndTime << std::endl;
    }
}

// Return the original JSON string back to C#
char* cstrResult = new char[strlen(jsonData) + 1];
strcpy_s(cstrResult, strlen(jsonData) + 1, jsonData);
return cstrResult;*/



