using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Замечания проверки уровня. Клик по замечанию ведёт к его стейджу и волне.
    public class IssuesPanel
    {
        private const string ROW_CLASS = "issue-row";
        private const string ERROR_ROW_CLASS = "issue-row--error";
        private const string WARNING_ROW_CLASS = "issue-row--warning";

        public event Action<LevelIssue> IssueChosen;

        private readonly ListView _issuesList;
        private readonly Label _title;
        private readonly List<LevelIssue> _issues = new();

        public IssuesPanel(VisualElement root)
        {
            _issuesList = root.Q<ListView>("issuesList");
            _title = root.Q<Label>("issuesTitle");

            _issuesList.itemsSource = _issues;
            _issuesList.makeItem = CreateIssueRow;
            _issuesList.bindItem = BindIssueRow;
            _issuesList.selectedIndicesChanged += OnSelectedIndicesChanged;
        }

        public void Refresh(IReadOnlyList<LevelIssue> issues)
        {
            _issues.Clear();
            _issues.AddRange(issues);

            _issuesList.SetSelectionWithoutNotify(Array.Empty<int>());
            _issuesList.Rebuild();

            _title.text = LevelEditorTexts.DescribeIssuesCount(issues);
        }

        private VisualElement CreateIssueRow()
        {
            Label row = new();
            row.AddToClassList(ROW_CLASS);

            return row;
        }

        private void BindIssueRow(VisualElement element, int index)
        {
            Label row = (Label)element;
            LevelIssue issue = _issues[index];

            row.text = LevelEditorTexts.FormatIssue(issue);
            row.tooltip = issue.ToString();
            row.EnableInClassList(ERROR_ROW_CLASS, issue.IsError);
            row.EnableInClassList(WARNING_ROW_CLASS, issue.IsError == false);
        }

        // Переход откладывается на кадр: окно при переходе перестраивает и этот список, а делать это
        // внутри его же события выбора нельзя.
        private void OnSelectedIndicesChanged(IEnumerable<int> indices)
        {
            int index = indices.DefaultIfEmpty(LevelAddress.NO_INDEX).First();

            if (index == LevelAddress.NO_INDEX)
                return;

            LevelIssue issue = _issues[index];
            _issuesList.schedule.Execute(() => IssueChosen?.Invoke(issue));
        }
    }
}
