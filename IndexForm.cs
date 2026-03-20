using DwSi.Dto;
using Newtonsoft.Json;
using System;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.Generic;
using log4net;
using RestSharp;
using System.Net;
using System.Xml;
using System.IO;
using System.Text;
using System.Diagnostics;


namespace DwSi
{
    public partial class IndexForm : Form
    {
        // 医保编码
        private string ybbm;

        private DwSi.service.Msh5ServiceClient client;

        private static readonly ILog log = LogManager.GetLogger(typeof(IndexForm));

        private Dictionary<string, string> deptMap;
        private Dictionary<string, string> xzMap;
        private Dictionary<string, string> tclbMap;
        private Dictionary<string, string> cbdMap;



        public IndexForm()
        {
            InitializeComponent();

            ToolTip gjPayToolTip = new ToolTip();
            gjPayToolTip.SetToolTip(gjPayButton, "身份证号请填写被共济人的身份证卡号！！！");

            ToolTip gjRefundToolTip = new ToolTip();
            gjRefundToolTip.SetToolTip(gjRefundButton, "订单号是消费是生成的id！！！");


            ToolTip cxZjToolTip = new ToolTip();
            cxZjToolTip.SetToolTip(cxZjButton, "请将{\"p_jshid\": \"地纬结算号\", \"p_tfsj\": \"退费时间\"} 保存在订单号文本框！！！");

            ToolTip gzCancelToolTip = new ToolTip();
            gzCancelToolTip.SetToolTip(cancelAccountButton, "请填写结算单号、二维码以及经办机构！！！");

            ToolTip cancelSettleToolTip = new ToolTip();
            gzCancelToolTip.SetToolTip(cancelPreButton, "请填写结算单号以及卡号！！！");
            gzCancelToolTip.SetToolTip(cancelSettleButton, "请填写结算单号以及卡号！！！");


            ToolTip itemToolTip = new ToolTip();
            itemToolTip.SetToolTip(itemButton, "请填写医院项目编码！！！");

            ToolTip dictToolTip = new ToolTip();
            dictToolTip.SetToolTip(itemButton, "请将代码编号填写在项目编码文本框！！！");

            ToolTip cancelZyToolTip = new ToolTip();
            cancelZyToolTip.SetToolTip(destroyZyButton, "请将住院号写在病历号文本框！！！");


            ToolTip cancelZySettleToolTip = new ToolTip();
            cancelZySettleToolTip.SetToolTip(destroyZySettleButton, "请将病历结算号填写在结算号文本框，住院号写在病历号文本框！！！");


            ToolTip settleReportToolTip = new ToolTip();
            settleReportToolTip.SetToolTip(settleReportButton, "请将结算号填写在结算号文本框！！！");

            deptMap = new Dictionary<string, string>();
            deptMap.Add("000000", "000000");
            deptMap.Add("省异地机构", "37000000");
            deptMap.Add("聊城市职工医疗保险事业处", "37150101");
            deptMap.Add("聊城市居民医疗保险事业处", "37150105");
            deptMap.Add("东昌府区职工医疗保险事业处", "37150201");
            deptMap.Add("东昌府区居民医疗保险事业处", "37150205");
            deptMap.Add("阳谷县职工医疗保险事业处", "37152101");
            deptMap.Add("阳谷县居民医疗保险事业处", "37152105");
            deptMap.Add("莘县职工医疗保险事业处", "37152201");
            deptMap.Add("莘县居民医疗保险事业处", "37152205");
            deptMap.Add("茌平县职工医疗保险事业处", "37152301");
            deptMap.Add("茌平县居民医疗保险事业处", "37152305");
            deptMap.Add("东阿县职工医疗保险事业处", "37152401");
            deptMap.Add("东阿县居民医疗保险事业处", "37152405");
            deptMap.Add("冠县职工医疗保险事业处", "37152501");
            deptMap.Add("冠县居民医疗保险事业处", "37152505");
            deptMap.Add("高唐县职工医疗保险事业处", "37152601");
            deptMap.Add("高唐县居民医疗保险事业处", "37152605");
            deptMap.Add("开发区企业", "37154001");
            deptMap.Add("聊城开发区城镇居民", "37154005");
            deptMap.Add("高新区企业", "37154101");
            deptMap.Add("聊城高新区城镇居民", "37154105");
            deptMap.Add("度假区企业", "37154201");
            deptMap.Add("聊城度假区城镇居民", "37154205");
            deptMap.Add("临清市职工医疗保险事业处", "37158101");
            deptMap.Add("临清市居民医疗保险事业处", "37158105");
            deptMap.Add("工伤东昌府区医疗保险处", "37159201");
            deptMap.Add("工伤阳谷县医疗保险处", "37159301");
            deptMap.Add("工伤莘县医疗保险处", "37159401");
            deptMap.Add("工伤茌平县医疗保险处", "37159501");
            deptMap.Add("工伤东阿县医保处", "37159601");
            deptMap.Add("工伤冠县医疗保险处", "37159701");
            deptMap.Add("工伤高唐县医疗保险处", "37159801");
            deptMap.Add("工伤聊城市医疗保险处", "37159901");
            deptMap.Add("工伤临清市医疗保险处", "37159A01");
            deptMap.Add("工伤聊城市开发区医疗保险处", "37159B01");
            deptMap.Add("工伤聊城市高新区医疗保险处", "37159C01");
            deptMap.Add("工伤聊城市度假区医疗保险处", "37159D01");


            xzMap = new Dictionary<string, string>();
            xzMap.Add("医疗", "C");
            xzMap.Add("工伤", "D");
            xzMap.Add("生育", "E");
            foreach(string xz in xzMap.Keys)
            {
                xzComboBox.Items.Add(xz);
            }


            // 0为仅获取人员基本信息，1为住院，4为门诊大病(特病)，6为普通门诊，不传时，默认值为0,其他具体值调用数据字典接口获取，代码编号：YLTCLB
            tclbMap = new Dictionary<string, string>();
            tclbMap.Add("住院", "1");
            tclbMap.Add("家庭病床", "2");
            tclbMap.Add("急诊转住院", "3");
            tclbMap.Add("门诊统筹", "4");
            tclbMap.Add("普通门诊", "6");
            foreach (string lb in tclbMap.Keys)
            {
                tclbComboBox.Items.Add(lb);
            }

            cbdMap = new Dictionary<string, string>();
            cbdMap.Add("济南市", "370100");
            cbdMap.Add("青岛市", "370200");
            cbdMap.Add("淄博市", "370300");
            cbdMap.Add("枣庄市", "370400");
            cbdMap.Add("东营市", "370500");
            cbdMap.Add("烟台市", "370600");
            cbdMap.Add("潍坊市", "370700");
            cbdMap.Add("济宁市", "370800");
            cbdMap.Add("泰安市", "370900");
            cbdMap.Add("威海市", "371000");
            cbdMap.Add("日照市", "371100");
            cbdMap.Add("临沂市", "371300");
            cbdMap.Add("德州市", "371400");
            cbdMap.Add("聊城市", "371500");
            cbdMap.Add("滨州市", "371600");
            cbdMap.Add("菏泽市", "371700");
            cbdMap.Add("省直", "379900");
            foreach (string cbd in cbdMap.Keys)
            {
                cbdComboBox.Items.Add(cbd);
            }
            // 默认参保地聊城市
            cbdComboBox.SelectedItem = "聊城市";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            client = new DwSi.service.Msh5ServiceClient();

            string resp = client.loginByYybm(ConfigurationManager.AppSettings["dwUser"], ConfigurationManager.AppSettings["dwPass"], ConfigurationManager.AppSettings["hospitalNo"]);
            if (string.IsNullOrEmpty(resp))
            {
                MessageBox.Show("登录失败！！！");
                return;
            }
            log.Info(resp);


            LoginResp loginResp = JsonConvert.DeserializeObject<LoginResp>(resp);
            if (loginResp.resultcode != 0)
            {
                MessageBox.Show("登录失败, code: " + loginResp.resultcode);
                return;
            }
            ybbm = loginResp.resulttext;

            // 默认 000000
            deptComboBox.SelectedIndex = 0;
        }


        private void InfoButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string idno = noTextBox.Text;
            if (string.IsNullOrEmpty(idno))
            {
                MessageBox.Show("请填写身份证号！！！");
                return;
            }
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_grbh", idno);

            // 选中的险种标志
            if (xzComboBox.SelectedItem != null)
            {
                string xzbz = xzComboBox.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(xzbz))
                {
                    foreach (var item in xzMap)
                    {
                        if (item.Key == xzbz)
                        {
                            param.Add("p_xzbz", item.Value);
                            break;
                        }
                    }
                }
            }
            // 选中的医疗统筹类别
            if (tclbComboBox.SelectedItem != null)
            {
                string yltclb = tclbComboBox.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(yltclb))
                {
                    foreach (var item in tclbMap)
                    {
                        if (item.Key == yltclb)
                        {
                            param.Add("p_yltclb", item.Value);
                            break;
                        }
                    }
                }
            }

            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "query_basic_info", JsonConvert.SerializeObject(param));
            Dictionary<string, string> map = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);

            if (map["resultcode"] != "0")
            {
                MessageBox.Show(JsonConvert.DeserializeObject(resp).ToString());
                return;
            }
            // 社保机构编号
            string sbjgbh = map["sbjgbh"];
            if (string.IsNullOrEmpty(sbjgbh))
            {
                MessageBox.Show("社保机构编号为空！！！");
            }

            // 社保机构名称
            string sbjgmc = "";
            foreach (var item in deptMap)
            {
                if (item.Value == sbjgbh)
                {
                    sbjgmc = item.Key;
                    break;
                }
            }
            if (string.IsNullOrEmpty(sbjgmc))
            {
                MessageBox.Show("找不到社保机构编号 " + sbjgbh + " 对应的社保机构名称！！！");
            }
            else
            {
                deptComboBox.Text = sbjgmc;
            }

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void GjButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string idno = noTextBox.Text;
            if (string.IsNullOrEmpty(idno))
            {
                MessageBox.Show("请填写被共济人身份证号！！！");
                return;
            }

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_grbh", idno);
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "query_basic_info", JsonConvert.SerializeObject(param));
            if (string.IsNullOrEmpty(resp))
            {
                MessageBox.Show("查找患者信息失败！！！");
                return;
            }
            Dictionary<string, string> map = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
            if (map["resultcode"] != "0")
            {
                MessageBox.Show(map["resulttext"]);
                param.Add("p_xm", "被共济人");
            }
            else
            {
                param.Add("p_xm", map["xm"]);
            }


            param.Remove("p_grbh");


            // 参保地编号
            string cbd = cbdComboBox.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(cbd))
            {
                foreach (var item in cbdMap)
                {
                    if (item.Key == cbd)
                    {
                        param.Add("p_cbdbm", item.Value);
                        break;
                    }
                }
            }


            param.Add("p_sfzhm", idno);

            param.Add("p_kh", "");
            param.Add("p_ewm", "");

            log.Info("read_ewm_yd 请求参数：" + JsonConvert.SerializeObject(param));
            resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "read_ewm_yd", JsonConvert.SerializeObject(param));
            log.Info("read_ewm_yd 响应：" + resp);
            map = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
            if (map["resultcode"] != "0")
            {
                MessageBox.Show(map["resulttext"]);
                return;
            }
            gjOrderIdTextBox.Text = map["sptddh"];
            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();

        }

        private void GjPayButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string idno = noTextBox.Text;
            if (string.IsNullOrEmpty(idno))
            {
                MessageBox.Show("请填写被共济人身份证号！！！");
                return;
            }
            string orderId = gjOrderIdTextBox.Text;
            if (string.IsNullOrEmpty(orderId))
            {
                MessageBox.Show("先调用共济查询接口获得订单ID！！！");
                return;
            }

            string amountStr = amountTextBox.Text;
            if (string.IsNullOrEmpty(amountStr))
            {
                MessageBox.Show("消费金额为空！！！");
                return;
            }

            Dictionary<string, string> param = new Dictionary<string, string>();

            // 参保地编号
            string cbd = cbdComboBox.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(cbd))
            {
                foreach (var item in cbdMap)
                {
                    if (item.Key == cbd)
                    {
                        param.Add("p_cbdbm", item.Value);
                        break;
                    }
                }
            }
            // 省平台订单号
            param.Add("p_sptddh", orderId);
            // 消费金额
            param.Add("p_zfje", amountStr);
            // 姓名
            param.Add("p_xm", "被共济人");
            // 身份证号码
            param.Add("p_sfzhm", idno);
            // 电子医保凭证认证标志 0：电子医保凭证认证，1：实体卡认证，默认为0
            param.Add("p_yxwmrz", "1");
            // 交易时间
            param.Add("p_jysj", DateTime.Now.ToString("yyyyMMddHHmmss"));

            log.Info("settle_gz_yd 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "settle_gz_yd", JsonConvert.SerializeObject(param));
            log.Info("settle_gz_yd 响应：" + resp);

            Dictionary<string, string> map = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
            if (map["resultcode"] != "0")
            {
                MessageBox.Show(map["resulttext"]);
                return;
            }
            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void GjRefundButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string idno = noTextBox.Text;
            if (string.IsNullOrEmpty(idno))
            {
                MessageBox.Show("请填写被共济人身份证号！！！");
                return;
            }
            string orderId = gjOrderIdTextBox.Text;
            if (string.IsNullOrEmpty(orderId))
            {
                MessageBox.Show("请填写消费订单号！！！");
                return;
            }

            string amountStr = amountTextBox.Text;
            if (string.IsNullOrEmpty(amountStr))
            {
                MessageBox.Show("退费金额为空！！！");
                return;
            }

            Dictionary<string, string> param = new Dictionary<string, string>();

            // 参保地编号
            string cbd = cbdComboBox.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(cbd))
            {
                foreach (var item in cbdMap)
                {
                    if (item.Key == cbd)
                    {
                        // 参保地编码
                        param.Add("p_cbdbm", item.Value);
                        break;
                    }
                }
            }
            // 姓名
            param.Add("p_xm", "被共济人");
            // 身份证号码
            param.Add("p_sfzhm", idno);

            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "read_ewm_yd", JsonConvert.SerializeObject(param));
            Dictionary<string, string> map = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
            if (map["resultcode"] != "0")
            {
                MessageBox.Show(map["resulttext"]);
                return;
            }
            string sptddh = map["sptddh"];
            if (string.IsNullOrEmpty(sptddh))
            {
                MessageBox.Show("获取省平台订单号失败！！！");
                return;
            }
            // 省平台订单号
            param.Add("p_sptddh", sptddh);
            // 结算号id
            param.Add("p_ysptddh", orderId);
            // 退费金额
            param.Add("p_tfje", amountStr);
            // 现金支付
            param.Add("p_xjzf", "0.0");
            // 电子医保凭证认证标志 0：电子医保凭证认证，1：实体卡认证，默认为0
            param.Add("p_yxwmrz", "1");
            // 交易时间
            param.Add("p_jysj", DateTime.Now.ToString("yyyyMMddHHmmss"));

            log.Info("destroy_gz_yd 请求参数：" + JsonConvert.SerializeObject(param));
            resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "destroy_gz_yd", JsonConvert.SerializeObject(param));
            log.Info("destroy_gz_yd 响应：" + resp);

            map = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
            if (map["resultcode"] != "0")
            {
                MessageBox.Show(map["resulttext"]);
                return;
            }
            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void AmountTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 允许数字、小数点和控制键
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // 只允许一个小数点
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void CxZjButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string cxJson = gjOrderIdTextBox.Text;
            if (string.IsNullOrEmpty(cxJson))
            {
                gjOrderIdTextBox.Text = "{\"p_jshid\": \"\", \"p_tfsj\": \"\"} ";
                return;
            }

            Dictionary<string, string> map = JsonConvert.DeserializeObject<Dictionary<string, string>>(cxJson);
            if (string.IsNullOrEmpty(map["p_jshid"]) || string.IsNullOrEmpty(map["p_tfsj"]))
            {
                MessageBox.Show("结合算号id或者计算时间为空！！！");
                return;
            }

            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "destroy_mz_xstz", cxJson);
            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void InfoCenterButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string idno = noTextBox.Text;
            if (string.IsNullOrEmpty(idno))
            {
                MessageBox.Show("请填写身份证号！！！");
                return;
            }

            var client = new RestClient("http://192.192.15.249:7001");

            var request = new RestRequest("/mhs5/mhs5Servlet", Method.GET);
            //request.OnBeforeDeserialization = res => RestSharpHelper.SetResponseEncoding(res, "gbk");

            request.AddParameter("_model_", "0");
            request.AddParameter("__bpoName__", "com.dw.mhs5.zhgl.Mhs5TestServiceBPO");
            request.AddParameter("__methodName__", "testService");

            string deptName = deptComboBox.SelectedItem.ToString();

            string xmlPara = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><p><s paraInJson=\"{{'shbzhm':'{0}'}}\"/><s yybm=\"{1}\" sbjgbh=\"{2}\" cservicename=\"QueryBasicInfo\"/></p>", idno, "250001", deptMap[deptName]);


            request.AddParameter("__xmlPara__", xmlPara);

            IRestResponse response = client.Execute(request);
            RestSharpHelper.SetResponseEncoding(response, "gbk");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                respRichTextBox.Text = FormatXml(response.Content);

                XmlDocument document = new XmlDocument();
                document.LoadXml(response.Content);

                cardTextBox.Text = document.SelectSingleNode("/p/d[last()]/r[@name='kh']/@value") == null ? "获取不到医保卡号" : document.SelectSingleNode("/p/d[last()]/r[@name='kh']/@value").Value;
            }
            else
            {
                respRichTextBox.Text = JsonConvert.SerializeObject(response);
            }
        }

        private string FormatXml(string data)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(data);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;
            try
            {
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = System.Xml.Formatting.Indented;
                xtw.Indentation = 1;
                xtw.IndentChar = '\t';
                document.WriteTo(xtw);
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
            }
            return sb.ToString();
        }

        private void ReadCardButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string cardNo = cardTextBox.Text;
            if (string.IsNullOrEmpty(cardNo))
            {
                MessageBox.Show("请填写卡号！！！");
                return;
            }

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_kh", cardNo);

            log.Info("read_card 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "read_card", JsonConvert.SerializeObject(param));
            log.Info("read_card 响应：" + resp);
            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();

        }

        private void DrugButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string idno = noTextBox.Text;
            if (string.IsNullOrEmpty(idno))
            {
                MessageBox.Show("请填写身份证号！！！");
                return;
            }

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_grbh", idno);
            // 查询最近 4 个月
            param.Add("p_qsrq", DateTime.Now.AddMonths(-4).ToString("yyyyMMdd"));
            param.Add("p_zzrq", DateTime.Now.ToString("yyyyMMdd"));

            log.Info("query_cbrjyxx 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "query_cbrjyxx", JsonConvert.SerializeObject(param));
            log.Info("query_cbrjyxx 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void CancelAccountButton_Click(object sender, EventArgs e)
        {
            string settleId = settleIdTextBox.Text;
            if (string.IsNullOrEmpty(settleId))
            {
                MessageBox.Show("请填写结算号！！！");
                return;
            }
            string ewm = ewmTextBox.Text;
            if (string.IsNullOrEmpty(ewm))
            {
                MessageBox.Show("请填写二维码！！！");
                return;
            }

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_jshid", settleId);
            param.Add("p_ewm", ewm);
            param.Add("p_ectoken", "370000ec31ur8p0dmq6c01500a0000072715f8");

            log.Info("destroy_ewmgz 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "destroy_ewmgz", JsonConvert.SerializeObject(param));
            log.Info("destroy_ewmgz 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void CancelPreButton_Click(object sender, EventArgs e)
        {
            string settleId = settleIdTextBox.Text;
            if (string.IsNullOrEmpty(settleId))
            {
                MessageBox.Show("请填写结算号！！！");
                return;
            }

            string cardNo = cardTextBox.Text;
            if (string.IsNullOrEmpty(cardNo))
            {
                MessageBox.Show("请填写卡号！！！");
                return;
            }

            string patientNo = cardNoTextBox.Text;

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_jshid", settleId);
            param.Add("p_kh", cardNo);
            param.Add("p_kl", ybbm);
            if (!string.IsNullOrEmpty(patientNo))
            {
                param.Add("p_blh", patientNo);
            }

            log.Info("destroy_mz_pre 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "destroy_mz_pre", JsonConvert.SerializeObject(param));
            log.Info("destroy_mz_pre 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();

        }

        private void CancelSettleButton_Click(object sender, EventArgs e)
        {
            string settleId = settleIdTextBox.Text;
            if (string.IsNullOrEmpty(settleId))
            {
                MessageBox.Show("请填写结算号！！！");
                return;
            }

            string cardNo = cardTextBox.Text;
            if (string.IsNullOrEmpty(cardNo))
            {
                MessageBox.Show("请填写卡号！！！");
                return;
            }


            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_jshid", settleId);
            param.Add("p_kh", cardNo);
            param.Add("p_kl", ybbm);

            string patientNo = cardNoTextBox.Text;
            if (!string.IsNullOrEmpty(patientNo))
            {
                param.Add("p_blh", patientNo);
            }

            log.Info("destroy_mz 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "destroy_mz", JsonConvert.SerializeObject(param));
            log.Info("destroy_mz 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void itemButton_Click(object sender, EventArgs e)
        {
            string itemNo = itemTextBox.Text;
            if (string.IsNullOrEmpty(itemNo))
            {
                MessageBox.Show("请填写医院项目编码！！！");
                return;
            }

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_yyxmbm", itemNo);
            param.Add("p_rq", DateTime.Now.ToString("yyyyMMdd"));

            

            log.Info("get_zfbl 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "get_zfbl", JsonConvert.SerializeObject(param));
            log.Info("get_zfbl 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void DictButton_Click(object sender, EventArgs e)
        {
            string itemNo = itemTextBox.Text;
            if (string.IsNullOrEmpty(itemNo))
            {
                MessageBox.Show("请在项目编号文本框填写代码编号！！！");
                return;
            }
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_dmbh", itemNo);



            log.Info("query_si_code 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "query_si_code", JsonConvert.SerializeObject(param));
            log.Info("query_si_code 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void ProjButton_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_filetype", "json");
            param.Add("p_sxh", "7013");


            log.Info("query_ml_by_sxh 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "query_ml_by_sxh", JsonConvert.SerializeObject(param));
            log.Info("query_ml_by_sxh 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void DestroyZyButton_Click(object sender, EventArgs e)
        {
            string patientNo = cardNoTextBox.Text;
            if (string.IsNullOrEmpty(patientNo))
            {
                MessageBox.Show("请在病历号填写住院号！！！");
                return;
            }
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_blh", patientNo);


            log.Info("destroy_zydj 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "destroy_zydj", JsonConvert.SerializeObject(param));
            log.Info("destroy_zydj 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void DestroyZySettleButton_Click(object sender, EventArgs e)
        {
            string patientNo = cardNoTextBox.Text;
            if (string.IsNullOrEmpty(patientNo))
            {
                MessageBox.Show("请在病历号填写住院号！！！");
                return;
            }

            string settleId = settleIdTextBox.Text;
            if (string.IsNullOrEmpty(settleId))
            {
                MessageBox.Show("请在结算号填写住院结算号！！！");
                return;
            }
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_jshid", settleId);
            param.Add("p_blh", patientNo);


            log.Info("destroy_zyjs 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "destroy_zyjs", JsonConvert.SerializeObject(param));
            log.Info("destroy_zyjs 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void SettleReportButton_Click(object sender, EventArgs e)
        {
            string settleId = settleIdTextBox.Text;
            if (string.IsNullOrEmpty(settleId))
            {
                MessageBox.Show("请在结算号填写病人结算号！！！");
                return;
            }
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_jshid", settleId);


            log.Info("print_jsd 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "print_jsd", JsonConvert.SerializeObject(param));
            log.Info("print_jsd 响应：" + resp);

            PdfResp pdfResp = JsonConvert.DeserializeObject<PdfResp>(resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();


            if (pdfResp.resultcode == 0)
            {
                byte[] buffer = System.Convert.FromBase64String(pdfResp.report);

                string tempDirectory = Path.GetTempPath();
                string fileName = $"{Guid.NewGuid()}.pdf";
                string filePath = Path.Combine(tempDirectory, fileName);

                File.WriteAllBytes(filePath, buffer);

                if (File.Exists(filePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }

            }
        }

        private void Label10_Click(object sender, EventArgs e)
        {
            MessageBox.Show(JsonConvert.SerializeObject(xzMap));
        }

        private void Label11_Click(object sender, EventArgs e)
        {
            MessageBox.Show(JsonConvert.SerializeObject(tclbMap));
        }

        private void Label4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(JsonConvert.SerializeObject(deptMap));
        }

        private void MbQueryButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string idno = noTextBox.Text;
            if (string.IsNullOrEmpty(idno))
            {
                MessageBox.Show("请填写身份证号！！！");
                return;
            }

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_grbh", idno);
            param.Add("p_cxlb", "1");
            // 查询最近 4 个月
            param.Add("p_qsrq", DateTime.Now.AddMonths(-3).ToString("yyyyMMdd"));
            param.Add("p_zzrq", DateTime.Now.ToString("yyyyMMdd"));

            log.Info("query_bryyqk 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "query_bryyqk", JsonConvert.SerializeObject(param));
            log.Info("query_bryyqk 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void CyReportButton_Click(object sender, EventArgs e)
        {
            string patientNo = cardNoTextBox.Text;
            if (string.IsNullOrEmpty(patientNo))
            {
                MessageBox.Show("请将住院号填写在病历号文本框！！！");
                return;
            }
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_blh", patientNo);


            log.Info("print_cyd 请求参数：" + JsonConvert.SerializeObject(param));
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "print_cyd", JsonConvert.SerializeObject(param));
            log.Info("print_cyd 响应：" + resp);

            PdfResp pdfResp = JsonConvert.DeserializeObject<PdfResp>(resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();


            if (pdfResp.resultcode == 0)
            {
                byte[] buffer = System.Convert.FromBase64String(pdfResp.report);

                string tempDirectory = Path.GetTempPath();
                string fileName = $"{Guid.NewGuid()}.pdf";
                string filePath = Path.Combine(tempDirectory, fileName);

                File.WriteAllBytes(filePath, buffer);

                if (File.Exists(filePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }

            }
        }

        private void label12_Click(object sender, EventArgs e)
        {
            MessageBox.Show(JsonConvert.SerializeObject(cbdMap));
        }

        private void InHospitalBtn_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();


            log.Info("query_bryyqk 请求参数：无");
            //string deptName = deptComboBox.SelectedItem.ToString();
            //string resp = client.invoke(deptMap[deptName], ybbm, Guid.NewGuid().ToString("N"), "query_zybrxx", "");
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "query_zybrxx", "");
            log.Info("query_zybrxx 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void policyButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("p_ylzcbs", "");

            log.Info("query_ylzcbs 请求参数：无");
            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "query_ylzcbs", JsonConvert.SerializeObject(param));
            log.Info("query_ylzcbs 响应：" + resp);

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void ZjPushButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string cxJson = gjOrderIdTextBox.Text;
            if (string.IsNullOrEmpty(cxJson))
            {
                MessageBox.Show("将推送json保存在订单号里！！！");
                return;
            }

            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "settle_mz_xstz", cxJson);
            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }

        private void QueryGzButton_Click(object sender, EventArgs e)
        {
            respRichTextBox.Clear();
            string orderId = gjOrderIdTextBox.Text;
            if (string.IsNullOrEmpty(orderId))
            {
                MessageBox.Show("将消费的时候返回的sptddh保存在订单号里！！！");
                return;
            }

            Dictionary<string, string> param = new Dictionary<string, string>();

            // 查询最近 4 个月
            param.Add("p_qsrq", DateTime.Now.AddMonths(-4).ToString("yyyyMMdd") + "000000");
            param.Add("p_zzrq", DateTime.Now.ToString("yyyyMMdd") + "235959");
            param.Add("p_sptddh", orderId);



            string resp = client.invoke(deptMap[deptComboBox.SelectedItem.ToString()], ybbm, Guid.NewGuid().ToString("N"), "query_jyxx_ydgz", JsonConvert.SerializeObject(param));
            if (string.IsNullOrEmpty(resp))
            {
                MessageBox.Show("查询共济消费信息失败！！！");
                return;
            }

            respRichTextBox.Text = JsonConvert.DeserializeObject(resp).ToString();
        }
    }
}
