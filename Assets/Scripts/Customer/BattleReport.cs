// BattleReport.cs - 战报记录类
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaskMerchantGame.CustomerSystem
{
    /// <summary>
    /// 检定结果
    /// </summary>
    public enum CheckResult
    {
        Success,        // 成功
        Failure,        // 失败
        WeaknessDeath   // 弱点触发死亡
    }

    /// <summary>
    /// 战报记录 - 记录顾客事件检定结果
    /// </summary>
    [Serializable]
    public class BattleReport
    {
        public int reportId;                        // 战报ID
        public DateTime timestamp;                  // 时间戳
        public CustomerInstance customer;           // 顾客实例
        public EventConfig eventConfig;             // 事件配置
        
        // 检定数据
        public FiveElementsAttributes finalAttributes;  // 最终检定属性（基础+面具）
        public int checkValue;                      // 检定值
        public int threshold;                       // 阈值
        public CheckResult result;                  // 检定结果
        
        // 死亡相关
        public bool isDead;                         // 是否死亡
        public DeathTag deathTag;                   // 死亡标签
        public string deathReason;                  // 死亡原因
        
        // 奖励
        public List<CarriedItem> lootItems;         // 掉落物品
        public int goldReward;                      // 金币奖励
        public int expReward;                       // 经验奖励

        public BattleReport()
        {
            reportId = 0;
            timestamp = DateTime.Now;
            lootItems = new List<CarriedItem>();
        }
    }

    /// <summary>
    /// 战报生成器 - 处理属性检定和战报生成
    /// </summary>
    public class BattleReportGenerator
    {
        private static int _reportIdCounter = 0;

        /// <summary>
        /// 生成战报 - 输入顾客实例，从事件池随机抽取匹配事件进行检定
        /// </summary>
        /// <param name="customer">顾客实例</param>
        /// <returns>战报记录</returns>
        public static BattleReport GenerateReport(CustomerInstance customer)
        {
            if (customer == null)
            {
                Debug.LogError("[BattleReportGenerator] 顾客实例为空");
                return null;
            }

            // 从事件池随机抽取事件
            EventConfig eventConfig = BattleEventManager.Instance.GetRandomEvent();
            if (eventConfig == null)
            {
                Debug.LogWarning("[BattleReportGenerator] 未找到匹配的事件");
                return null;
            }

            return GenerateReport(customer, eventConfig);
        }

        /// <summary>
        /// 生成战报 - 使用指定事件
        /// </summary>
        /// <param name="customer">顾客实例</param>
        /// <param name="eventConfig">事件配置</param>
        /// <returns>战报记录</returns>
        public static BattleReport GenerateReport(CustomerInstance customer, EventConfig eventConfig)
        {
            if (customer == null || eventConfig == null)
            {
                Debug.LogError("[BattleReportGenerator] 参数为空");
                return null;
            }

            BattleReport report = new BattleReport
            {
                reportId = ++_reportIdCounter,
                timestamp = DateTime.Now,
                customer = customer,
                eventConfig = eventConfig
            };

            // 获取面具属性加成
            report.finalAttributes = customer.GetMaskAttributes();

            // 获取检定元素的属性值
            FiveElements checkElement = eventConfig.checkElement;
            report.checkValue = report.finalAttributes.GetValue(checkElement);
            report.threshold = eventConfig.checkThreshold;

            // 进行属性检定
            PerformCheck(report, customer);

            // 处理结果
            ProcessResult(report, customer);

            Debug.Log($"[BattleReport] 生成战报: {report.eventConfig.eventName}, " +
                      $"检定值:{report.checkValue}, 阈值:{report.threshold}, " +
                      $"结果:{report.result}, 死亡:{report.isDead}");

            return report;
        }

        /// <summary>
        /// 执行属性检定
        /// </summary>
        private static void PerformCheck(BattleReport report, CustomerInstance customer)
        {
            FiveElements weakness = customer.GetTrueWeakness();
            FiveElements checkElement = report.eventConfig.checkElement;

            // 检查弱点属性是否为负
            int weaknessValue = report.finalAttributes.GetValue(weakness);
            
            // 当弱点属性为负时，一定触发弱点Tag，顾客死亡
            if (weaknessValue < 0)
            {
                report.result = CheckResult.WeaknessDeath;
                report.isDead = true;
                report.deathTag = DeathTag.WeaknessTriggered;
                report.deathReason = $"弱点属性({weakness})为负值({weaknessValue})，触发弱点死亡";
                return;
            }

            // 正常检定：检定值 >= 阈值 则成功
            if (report.checkValue >= report.threshold)
            {
                report.result = CheckResult.Success;
                report.isDead = false;
                report.deathTag = DeathTag.None;
                report.deathReason = string.Empty;
            }
            else
            {
                report.result = CheckResult.Failure;
                report.isDead = true;
                report.deathTag = report.eventConfig.deathTag;
                report.deathReason = report.eventConfig.deathDescription;
            }
        }

        /// <summary>
        /// 处理检定结果
        /// </summary>
        private static void ProcessResult(BattleReport report, CustomerInstance customer)
        {
            if (report.isDead)
            {
                // 设置奖励
                report.goldReward = report.eventConfig.goldReward;
                report.expReward = report.eventConfig.expReward;

                // 标记顾客死亡
                customer.SetDead(report.deathTag, report.deathReason);
            }
            else
            {
                // 顾客存活，无掉落
                report.lootItems.Clear();
                report.goldReward = 0;
                report.expReward = 0;
            }
        }

        /// <summary>
        /// 获取战报摘要
        /// </summary>
        public static string GetReportSummary(BattleReport report)
        {
            if (report == null) return "无效战报";

            string summary = $"=== 战报 #{report.reportId} ===\n";
            summary += $"顾客: {report.customer.GetCustomerName()}\n";
            summary += $"事件: {report.eventConfig.eventName}\n";
            summary += $"检定属性: {report.eventConfig.checkElement}\n";
            summary += $"检定值: {report.checkValue} / 阈值: {report.threshold}\n";
            summary += $"最终属性: {report.finalAttributes}\n";
            summary += $"结果: {report.result}\n";

            if (report.isDead)
            {
                summary += $"死亡标签: {report.deathTag}\n";
                summary += $"死亡原因: {report.deathReason}\n";
                summary += $"掉落物品: {report.lootItems.Count}件\n";
                foreach (var item in report.lootItems)
                {
                    summary += $"  - {item.itemName} x{item.quantity}\n";
                }
                summary += $"金币奖励: {report.goldReward}\n";
                summary += $"经验奖励: {report.expReward}\n";
            }

            return summary;
        }
    }

    /// <summary>
    /// 战报管理器 - 管理所有战报记录
    /// </summary>
    public class BattleReportManager : MonoBehaviour
    {
        #region 单例

        private static BattleReportManager _instance;
        public static BattleReportManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BattleReportManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("BattleReportManager");
                        _instance = go.AddComponent<BattleReportManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        #endregion

        private List<BattleReport> _reports = new List<BattleReport>();

        /// <summary>
        /// 顾客死亡事件
        /// </summary>
        public event Action<BattleReport> OnCustomerDeath;

        /// <summary>
        /// 战报生成事件
        /// </summary>
        public event Action<BattleReport> OnReportGenerated;

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
            }
        }

        /// <summary>
        /// 为顾客生成战报
        /// </summary>
        public BattleReport GenerateReportForCustomer(CustomerInstance customer)
        {
            BattleReport report = BattleReportGenerator.GenerateReport(customer);
            if (report != null)
            {
                _reports.Add(report);
                OnReportGenerated?.Invoke(report);

                if (report.isDead)
                {
                    OnCustomerDeath?.Invoke(report);
                }
            }
            return report;
        }

        /// <summary>
        /// 为顾客生成战报（指定事件）
        /// </summary>
        public BattleReport GenerateReportForCustomer(CustomerInstance customer, EventConfig eventConfig)
        {
            BattleReport report = BattleReportGenerator.GenerateReport(customer, eventConfig);
            if (report != null)
            {
                _reports.Add(report);
                OnReportGenerated?.Invoke(report);

                if (report.isDead)
                {
                    OnCustomerDeath?.Invoke(report);
                }
            }
            return report;
        }

        /// <summary>
        /// 获取所有战报
        /// </summary>
        public List<BattleReport> GetAllReports()
        {
            return new List<BattleReport>(_reports);
        }

        /// <summary>
        /// 获取指定顾客的战报
        /// </summary>
        public BattleReport GetReportForCustomer(CustomerInstance customer)
        {
            foreach (var report in _reports)
            {
                if (report.customer == customer)
                    return report;
            }
            return null;
        }

        /// <summary>
        /// 清空战报
        /// </summary>
        public void ClearReports()
        {
            _reports.Clear();
        }
    }
}
