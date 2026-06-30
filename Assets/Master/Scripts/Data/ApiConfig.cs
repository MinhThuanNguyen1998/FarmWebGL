using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ApiConfig
{
    // API
    //public static string API_AUTH_URL = "http://localhost:3000/api/login";
    //public static string API_DATA_URL = "http://localhost:3000/api/data";
    //public static string API_INVENTORY_URL = "http://localhost:3000/api/inventory";

    public static string API_AUTH_URL = "https://homagame.com/api/login";
    public static string API_DATA_URL = "https://homagame.com/api/data-user";
    public static string API_INVENTORY_URL = "https://homagame.com/api/inventory";


    public static string API_GET_USER_DATA_URL => API_DATA_URL;
    public static string API_ADD_ANIMAL_URL => API_DATA_URL;
    public static string API_GET_INVENTORY_URL => API_INVENTORY_URL;
}



