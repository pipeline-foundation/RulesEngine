﻿// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;

namespace DemoApp
{
    public class BasicDemo
    {
        public void Run()
        {
            Console.WriteLine($"Running {nameof(BasicDemo)}....");
            List<WorkflowRules> workFlowRules = new List<WorkflowRules>();
            WorkflowRules workflowRule = new WorkflowRules();
            workflowRule.WorkflowName = "Test Workflow Rule 1";

            List<Rule> rules = new List<Rule>();

            Rule rule = new Rule();
            rule.RuleName = "Test Rule";
            rule.SuccessEvent = "Count is within tolerance.";
            rule.ErrorMessage = "Over expected.";
            rule.Expression = "count < 3";
            rule.RuleExpressionType = RuleExpressionType.LambdaExpression;

            rules.Add(rule);

            workflowRule.Rules = rules;

            workFlowRules.Add(workflowRule);

            var bre = new RulesEngine.RulesEngine(workFlowRules.ToArray(), null);

            dynamic datas = new ExpandoObject();
            datas.count = 1;
            var inputs = new dynamic[]
              {
                    datas
              };

            List<RuleResultTree> resultList = bre.ExecuteAllRulesAsync("Test Workflow Rule 1", inputs).Result;

            bool outcome = false;

            //Different ways to show test results:
            outcome = resultList.TrueForAll(r => r.IsSuccess);

            resultList.OnSuccess((eventName) => {
                Console.WriteLine($"Result '{eventName}' is as expected.");
                outcome = true;
            });

            resultList.OnFail(() => {
                outcome = false;
            });

            Console.WriteLine($"Test outcome: {outcome}.");
        }
    }
}
