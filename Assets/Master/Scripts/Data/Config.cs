using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config 
{
    public static string LoginProcessing = "Logging in...";
    public static string LoginEmptyFields = "Please fill in all fields";
    public static string LoginFailed = "Incorrect login information";
    public static string LoginSuccess = "Login successful";
    public static string ServerError = "Error connecting to server";
    public static string DataLoadError = "Failed to load data. Please try again.";

    // Scene names
    public static string Main_Scene = "Main";
    public static string Login_Scene = "Login";
    public static string Bootstrap_Scene = "Bootstrap";


    // Reward
    public static string RewardSuccess = "Reward claimed successfully!";
    public static string RewardAlreadyClaimed = "Reward has already been\n claimed today";
    public static string RewardFailed = "Failed to claim reward due to a \nsystem error";

    // Add Animal
    public static string AddAnimalSuccess = "Animal added successfully";
    public static string AddAnimalFailed = "Failed to add animal";

    // Boss Challenge
    public static string BossChallengeComingSoon = "Coming soon...";

    // Logout
    public static string Logout = "Do you want to return to \nthe login screen?";
}

