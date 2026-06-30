using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ApiRewardResponse
{
    public bool status;
    public string message;
    public RewardData data;
}
[Serializable]
public class RewardData
{
    public bool can_claim;
    public string reward_amount;
    public string total_amount_user;
    public string date;
}
