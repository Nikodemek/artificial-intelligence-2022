from ast import arg
from audioop import avg
import json
import os
from typing import Type
import numpy as np
from scipy.optimize import curve_fit
from matplotlib import pyplot as plt

class TestResult(object):
    def __init__(
        self,
        input_data : list[list[float]],
        expected_results : list[float],
        actual_results : list[float],
        guessing_accuracy : float,
        guessing_class_accuracy : dict[str, int],
        entire_error : list[float],
        individual_errors : list[list[float]],
        output_neurons_weights : list[list[float]],
        hidden_neurons_values : list[list[float]],
        hidden_neurons_weights : list[list[list[float]]]):
        self.input_data = input_data
        self.expected_results = expected_results,
        self.actual_results = actual_results,
        self.guessing_accuracy = guessing_accuracy,
        self.guessing_class_accuracy = guessing_class_accuracy,
        self.entire_error = entire_error,
        self.individual_errors = individual_errors,
        self.output_neurons_weights = output_neurons_weights,
        self.hidden_neurons_values = hidden_neurons_values,
        self.hidden_neurons_weights = hidden_neurons_weights
        pass


dirName = os.path.dirname(__file__)

def read_file(filename : str):
    with open(os.path.join(dirName, filename)) as dataFile:
        return dataFile.read()

def as_test_result(dct):
    return TestResult(
        dct['InputData'],
        dct['ExpectedResults'],
        dct['ActualResults'],
        dct['GuessingAccuracy'],
        dct['GuessingClassAccuracy'],
        dct['EntireError'],
        dct['IndividualErrors'],
        dct['OutputNeuronsWeights'],
        dct['HiddenNeuronsValues'],
        dct['HiddenNeuronsWeights'])

def lines_to_list(file_data : str, type : Type):
    data = []
    for line in file_data.split('\n'):
        try:
            casted = type(line)
            data.append(casted)
        except:
            pass
    return data

def normalize_data(data : list[float], tolerance : float):
    normalized = []
    max_elements = 2 # len(data) / 10
    queue = []
    queue.append(data[0])
    for val in data:
        average = np.average(queue)
        if val > average + tolerance or val < average - tolerance:
            continue
        normalized.append(val)
        queue.append(val)
        if len(queue) >= max_elements:
            queue.pop(0)
    return normalized


def main():
    # rawJson = read_file("result.json")
    # results : TestResult = as_test_result(json.loads(rawJson))
    
    errors = lines_to_list(read_file('errors.txt'), float)
    errors = normalize_data(errors,10)
    arguments = range(0, len(errors))
    
    mymodel = np.poly1d(np.polyfit(arguments, errors, 20))
    myline = np.linspace(arguments[0], arguments[-1], 100)

    fig, ax = plt.subplots(figsize = (6, 6))
    ax.scatter(arguments, errors, alpha=0.7, s=10)
    ax.plot(myline, mymodel(myline), color='red')
    ax.set_xlabel('Epoch')
    ax.set_ylabel('Error')
    plt.show()












if (__name__=='__main__'):
    main()