using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApp4.Core;

namespace WpfApp4.Modules.MemberQuery;

public partial class MemberQueryViewModel : ObservableObject
{
    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private ObservableCollection<Member> _members = new();

    private List<Member> _allMembers = new();

    public MemberQueryViewModel()
    {
        LoadMockData();
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterMembers();
    }

    [RelayCommand]
    private void Search()
    {
        FilterMembers();
    }

    [RelayCommand]
    private void Reset()
    {
        SearchText = "";
        FilterMembers();
    }

    [RelayCommand]
    private void Logout()
    {
        NavigationService.NavigateTo("Login");
    }

    private void LoadMockData()
    {
        _allMembers = new List<Member>
        {
            new Member { Id = 1, Name = "张三", Phone = "13800138001", Level = "金牌", RegisterDate = new DateTime(2023, 1, 15) },
            new Member { Id = 2, Name = "李四", Phone = "13800138002", Level = "银牌", RegisterDate = new DateTime(2023, 3, 20) },
            new Member { Id = 3, Name = "王五", Phone = "13800138003", Level = "普通", RegisterDate = new DateTime(2023, 5, 10) },
            new Member { Id = 4, Name = "赵六", Phone = "13800138004", Level = "金牌", RegisterDate = new DateTime(2023, 7, 8) },
            new Member { Id = 5, Name = "孙七", Phone = "13800138005", Level = "银牌", RegisterDate = new DateTime(2023, 9, 12) },
        };
        FilterMembers();
    }

    private void FilterMembers()
    {
        Members.Clear();
        var query = string.IsNullOrWhiteSpace(SearchText)
            ? _allMembers
            : _allMembers.Where(m => m.Name.Contains(SearchText) || m.Phone.Contains(SearchText));

        foreach (var member in query)
        {
            Members.Add(member);
        }
    }
}
