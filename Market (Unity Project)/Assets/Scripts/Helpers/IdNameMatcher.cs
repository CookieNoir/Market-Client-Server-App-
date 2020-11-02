using System.Collections.Generic;

public static class IdNameMatcher
{
    public static List<string> itemNames = new List<string>
    {
        "Item", // itemNames[0], Placeholder
        // Names below should match the database names
        "Дерево",
        "Камень",
        "Кожа",
        "Чеснок",
        "Шерсть",
        "Сырое мясо",
        "Каменная лопата",
        "Каменный топор",
        "Жареное мясо",
        "Чесночный соус",
        "Деревянный стол",
        "Кресло",
        "Деревянный стул",
        "Кровать",
        "Деревянный шкаф",
    };

    public static List<string> typeNames = new List<string>
    {
        "Type", // typeNames[0], Placeholder
        // Names below should match the database names
        "Материал",
        "Инструмент",
        "Пища",
        "Мебель",
    };

    public static Dictionary<int, int> itemTypeMatch = new Dictionary<int, int>
    {
        { 0, 0 },
        { 1, 1 },
        { 2, 1 },
        { 3, 1 },
        { 4, 1 },
        { 5, 1 },
        { 6, 1 },
        { 7, 2 },
        { 8, 2 },
        { 9, 3 },
        { 10, 3 },
        { 11, 4 },
        { 12, 4 },
        { 13, 4 },
        { 14, 4 },
        { 15, 4 },
    };
}
