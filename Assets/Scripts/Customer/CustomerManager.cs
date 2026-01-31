// CustomerManager.cs - 顾客管理器
using System;
using System.Collections.Generic;
using UnityEngine;
using ConfigData;

namespace MaskMerchantGame.CustomerSystem
{
    /// <summary>
    /// 顾客管理器 - 管理每日顾客实例
    /// </summary>
    public class CustomerManager : MonoBehaviour
    {
        #region 单例

        private static CustomerManager _instance;
        public static CustomerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CustomerManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("CustomerManager");
                        _instance = go.AddComponent<CustomerManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 配置

        [Header("每日顾客配置")]
        [SerializeField] private int customersPerDay = 5;           // 每天顾客数量

        #endregion

        #region 私有字段

        private int _currentDay = 1;                                // 当前天数
        private int _currentCustomerIndex = 0;                      // 当前顾客索引
        private List<CustomerInstance> _todayCustomers;             // 今日顾客列表
        private Dictionary<int, List<CustomerInstance>> _dailyCustomerHistory;  // 每日顾客历史记录

        #endregion

        #region 事件

        /// <summary>
        /// 新的一天开始事件
        /// </summary>
        public event Action<int> OnNewDayStarted;

        /// <summary>
        /// 顾客列表初始化完成事件
        /// </summary>
        public event Action<List<CustomerInstance>> OnCustomersInitialized;

        /// <summary>
        /// 当前顾客变更事件
        /// </summary>
        public event Action<CustomerInstance, int> OnCurrentCustomerChanged;

        #endregion

        #region Unity生命周期

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _todayCustomers = new List<CustomerInstance>();
            _dailyCustomerHistory = new Dictionary<int, List<CustomerInstance>>();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化新的一天，生成当日顾客
        /// </summary>
        /// <param name="day">天数</param>
        public void InitializeDay(int day)
        {
            _currentDay = day;
            _currentCustomerIndex = 0;
            _todayCustomers.Clear();

            // 生成当日顾客实例
            GenerateDailyCustomers();

            // 保存到历史记录
            _dailyCustomerHistory[day] = new List<CustomerInstance>(_todayCustomers);

            Debug.Log($"[CustomerManager] 第{day}天初始化完成，生成{_todayCustomers.Count}个顾客");

            OnNewDayStarted?.Invoke(day);
            OnCustomersInitialized?.Invoke(_todayCustomers);
        }

        /// <summary>
        /// 初始化新的一天（使用指定的配置ID列表）
        /// </summary>
        /// <param name="day">天数</param>
        /// <param name="configIds">顾客配置ID列表</param>
        public void InitializeDayWithConfigs(int day, int[] configIds)
        {
            _currentDay = day;
            _currentCustomerIndex = 0;
            _todayCustomers.Clear();

            foreach (int configId in configIds)
            {
                Customer configData = ConfigData.CustomerManager.GetConfig(configId);
                if (configData != null)
                {
                    CustomerInstance instance = new CustomerInstance(configData);
                    _todayCustomers.Add(instance);
                }
                else
                {
                    Debug.LogWarning($"[CustomerManager] 未找到顾客配置: ID={configId}");
                }
            }

            // 保存到历史记录
            _dailyCustomerHistory[day] = new List<CustomerInstance>(_todayCustomers);

            Debug.Log($"[CustomerManager] 第{day}天初始化完成，生成{_todayCustomers.Count}个顾客");

            OnNewDayStarted?.Invoke(day);
            OnCustomersInitialized?.Invoke(_todayCustomers);
        }

        /// <summary>
        /// 获取当日顾客列表
        /// </summary>
        public List<CustomerInstance> GetTodayCustomers()
        {
            return new List<CustomerInstance>(_todayCustomers);
        }

        /// <summary>
        /// 获取指定天数的顾客列表
        /// </summary>
        /// <param name="day">天数</param>
        public List<CustomerInstance> GetCustomersForDay(int day)
        {
            if (_dailyCustomerHistory.TryGetValue(day, out var customers))
            {
                return new List<CustomerInstance>(customers);
            }
            return new List<CustomerInstance>();
        }

        /// <summary>
        /// 按索引获取当日顾客
        /// </summary>
        /// <param name="index">顾客索引</param>
        /// <returns>顾客实例</returns>
        public CustomerInstance GetCustomerByIndex(int index)
        {
            if (index >= 0 && index < _todayCustomers.Count)
            {
                return _todayCustomers[index];
            }
            Debug.LogWarning($"[CustomerManager] 无效的顾客索引: {index}");
            return null;
        }

        /// <summary>
        /// 获取当前顾客
        /// </summary>
        public CustomerInstance GetCurrentCustomer()
        {
            return GetCustomerByIndex(_currentCustomerIndex);
        }

        /// <summary>
        /// 获取当前顾客索引
        /// </summary>
        public int GetCurrentCustomerIndex()
        {
            return _currentCustomerIndex;
        }

        /// <summary>
        /// 设置当前顾客索引
        /// </summary>
        /// <param name="index">顾客索引</param>
        public void SetCurrentCustomerIndex(int index)
        {
            if (index >= 0 && index < _todayCustomers.Count)
            {
                _currentCustomerIndex = index;
                OnCurrentCustomerChanged?.Invoke(_todayCustomers[index], index);
            }
        }

        /// <summary>
        /// 切换到下一个顾客
        /// </summary>
        /// <returns>是否成功切换</returns>
        public bool NextCustomer()
        {
            if (_currentCustomerIndex < _todayCustomers.Count - 1)
            {
                _currentCustomerIndex++;
                OnCurrentCustomerChanged?.Invoke(_todayCustomers[_currentCustomerIndex], _currentCustomerIndex);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 切换到上一个顾客
        /// </summary>
        /// <returns>是否成功切换</returns>
        public bool PreviousCustomer()
        {
            if (_currentCustomerIndex > 0)
            {
                _currentCustomerIndex--;
                OnCurrentCustomerChanged?.Invoke(_todayCustomers[_currentCustomerIndex], _currentCustomerIndex);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当日顾客数量
        /// </summary>
        public int GetTodayCustomerCount()
        {
            return _todayCustomers.Count;
        }

        /// <summary>
        /// 获取当前天数
        /// </summary>
        public int GetCurrentDay()
        {
            return _currentDay;
        }

        /// <summary>
        /// 检查是否还有下一个顾客
        /// </summary>
        public bool HasNextCustomer()
        {
            return _currentCustomerIndex < _todayCustomers.Count - 1;
        }

        /// <summary>
        /// 检查是否还有上一个顾客
        /// </summary>
        public bool HasPreviousCustomer()
        {
            return _currentCustomerIndex > 0;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 生成当日顾客
        /// </summary>
        private void GenerateDailyCustomers()
        {
            var allConfigs = ConfigData.CustomerManager.GetAllConfigs();
            var configList = new List<Customer>();
            foreach (var kvp in allConfigs)
            {
                configList.Add(kvp.Value);
            }

            if (configList.Count == 0)
            {
                Debug.LogWarning("[CustomerManager] 没有可用的顾客配置");
                return;
            }

            // 随机选择顾客
            int count = Mathf.Min(customersPerDay, configList.Count);
            for (int i = 0; i < count && configList.Count > 0; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, configList.Count);
                CustomerInstance instance = new CustomerInstance(configList[randomIndex]);
                _todayCustomers.Add(instance);
                configList.RemoveAt(randomIndex);
            }
        }

        #endregion
    }
}
