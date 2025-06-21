using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSeziDateTime : MonoBehaviour
{
    [SerializeField]
    private SerializedDateTime savedTime; // Dùng để test

    private void Start()
    {
        // Nếu chưa có thời gian nào được lưu (ticks == 0), gán thời gian hiện tại
        if (savedTime.DateTime.Ticks == 0)
        {
            savedTime.DateTime = DateTime.Now;
            Debug.Log($"[Init] Lưu thời gian hiện tại: {savedTime.DateTime}");
        }
    }

    private void Update()
    {
        // Kiểm tra nếu đã hơn 3 giờ kể từ thời điểm lưu
        if (DateTime.Now > savedTime.DateTime.AddHours(3))
        {
            Debug.Log("Đã qua 3 giờ kể từ thời điểm lưu.");
        }
        else
        {
            TimeSpan diff = DateTime.Now - savedTime.DateTime;
            Debug.Log($"Thời gian trôi qua: {diff.TotalMinutes:F1} phút");
        }
    }
}
