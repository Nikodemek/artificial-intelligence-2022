import numpy as num
from matplotlib import pyplot as plt
import pandas as pd
import os

CSVData = pd.read_csv('data.csv', encoding = "utf-8")
Depths = CSVData['depth'].unique()
X_axis = num.arange(len(Depths))

def get_data(criteria : str, subjects: list, column: str, additional = 'none'):
    list = []
    
    for subject in subjects:
        inner_list = []
        for depth in Depths:
            depth_split = CSVData[CSVData['depth'] == depth]
            filtered_data = depth_split[CSVData[column] == subject]
            if additional != 'none':
                filtered_data = filtered_data[CSVData['method'] == additional]
            mean_value = num.mean(filtered_data[criteria])
            inner_list.append(mean_value)
        list.append(inner_list)
        
    return list

def main():
    scale = 2
    row_size = 2
    column_size = 2

    titles = [
        'Ogółem',
        'A*',
        'BFS',
        'DFS',
    ]

    criteria = [
        'path_length',
        'visited',
        'processed',
        'max_depth',
        'execution_time'
    ]

    criteria_switcher = {
        'path_length': "Długość znalezionego rozwiązania",
        'visited': "Liczba stanów odwiedzonych",
        'processed': "Liczba stanów przetworzonych",
        'max_depth': "Maksymalna osiągnięta głębokość rekursji",
        'execution_time': "Czas trwania procesu obliczeniowego [ms]"
    }

    strategy_switcher = {
        0: CSVData['method'].unique(), 
        1: CSVData[CSVData['method'] == 'astr']['strategy'].unique(),
        2: CSVData[CSVData['method'] == 'bfs']['strategy'].unique(),
        3: CSVData[CSVData['method'] == 'dfs']['strategy'].unique()
    }

    column_switcher = {
        0: 'method',
        1: 'strategy',
        2: 'strategy',
        3: 'strategy'
    }

    additional_switcher = {
        0: 'none',
        1: 'none',
        2: 'bfs',
        3: 'dfs'
    }

    width_switcher = {
        0: .25,
        1: .3,
        2: .1,
        3: .1
    }

    for criterion in criteria:

        fig, axs = plt.subplots(row_size, column_size, figsize=(5 * scale, 3.5 * scale))
        fig.suptitle(criteria_switcher.get(criterion, "Invalid label"), fontsize=16)
        x, y = 0, 0

        for i, title in enumerate(titles):
            strategies = strategy_switcher.get(i, "Invalid label")
            column = column_switcher.get(i, "Invalid label")
            additional = additional_switcher.get(i, "Invalid label")
            width = width_switcher.get(i, "Invalid label")

            list = get_data(criterion, strategies, column, additional)
            
            for i, data in enumerate(list):
                if len(list) % 2 == 0:
                    position = X_axis-(width*.5 + width * (len(list)/2 - 1 - i))
                else:
                    position = X_axis - width + (width * i)
                axs[x,y].bar(position, data, width, label=strategies[i])  

            axs[x,y].set_xticks(X_axis, Depths)
            if y == 0:
                axs[x,y].set_ylabel(criteria_switcher.get(criterion, "Invalid label"))
            if x == column_size - 1:
                axs[x,y].set_xlabel("Głębokość")
            axs[x,y].set_title(title)
            if not x == column_size - 1 & y == row_size - 1:
                axs[x,y].legend()

            if y == row_size - 1:
                y = 0
                x += 1
            else:
                y += 1

    fig.tight_layout()
    plt.show()


if (__name__=="__main__"):
    main()
