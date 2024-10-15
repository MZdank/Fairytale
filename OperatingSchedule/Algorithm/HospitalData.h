#pragma once
// HospitalData.h
#ifndef HOSPITALDATA_H
#define HOSPITALDATA_H

#include "pch.h"
#include <string>
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


#endif
