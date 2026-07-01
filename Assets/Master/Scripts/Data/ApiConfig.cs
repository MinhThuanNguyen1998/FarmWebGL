using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ApiConfig
{
    // API
    //public static string API_AUTH_URL = "http://localhost:3000/api/login";
    //public static string API_DATA_URL = "http://localhost:3000/api/data";
    //public static string API_INVENTORY_URL = "http://localhost:3000/api/inventory";
    //public static string API_CLAIM_REWARD_URL = "https://homagame.com/api/claim-reward";

    public static string API_AUTH_URL = "https://homagame.com/api/login";
    public static string API_DATA_URL = "https://homagame.com/api/data-user";
    public static string API_INVENTORY_URL = "https://homagame.com/api/inventory";
    public static string API_CLAIM_REWARD_URL = "https://homagame.com/api/daily-reward";

    public static string API_ADD_ANIMAL_URL = "https://homagame.com/api/farm/add-animal";
    public static string API_LOAD_ANIMAL_URL = "https://homagame.com/api/barn/load-animal";


    // Auth API
    public static string API_GET_USER_DATA_URL => API_DATA_URL;

    // Inventory API
    public static string API_GET_INVENTORY_URL => API_INVENTORY_URL;

    // Claim Reward API
    public static string API_POST_CLAIM_REWARD => API_CLAIM_REWARD_URL;

    // Animal API
    public static string API_POST_ADD_ANIMAL_URL => API_ADD_ANIMAL_URL;
    public static string API_GET_LOAD_ANIMAL_URL => API_LOAD_ANIMAL_URL;
}



