import os
import time

import numpy as np
import mne
import neurodecode.utils.q_common as qc
from neurodecode.utils import pycnbi_utils as pu
from neurodecode.stream_receiver.stream_receiver import StreamReceiver
from mne.preprocessing import (ICA, create_eog_epochs, create_ecg_epochs,
                               corrmap)

paradigm = ''

def process_stream(sr, EEG_CH_NAMES, unused_channels, info, filter_values,
                   reference_ica, band):
        sr.acquire()  # Read data from LSL
        window, tslist = sr.get_window()  # window = [samples x channels]
        window = window.T  # channels x samples
        #Raw
        raw_array = mne.io.RawArray(window, info)
        raw_array.drop_channels(ch_names=unused_channels)
        raw_array.set_montage('standard_1005',raise_if_subset=False)
        # filter out
        raw_array.notch_filter(filter_values)
        '''
        ica = ICA(n_components=0.80, method='fastica').fit(raw_array)
        #icas = [reference_ica] + [ica]
        #out = corrmap(icas, template=(0,0), label="blinks",
        #          show=False, plot=False, threshold=.8, ch_type='eeg')
        #corrmap(icas, template=(0,1), label="horizontal",
                show=False, plot=False, threshold=.8, ch_type='eeg')
        #exclude = ica.labels_['blinks'] + ica.labels_['horizontal']
        if len(ica.labels_['blinks']) >= 1:
            print('blink')
        if len(ica.labels_['horizontal']) >= 1:
            print('horizontal')
        #ica.apply(raw_array, exclude=exclude)
        '''
        #take only the last second of data
        raw_array.crop(4,)
        # compute psd
        psds,freqs = mne.time_frequency.psd_welch(raw_array,fmin=8,fmax=12)
        #psds = 10. * np.log10(psds)
        psds_mean = psds.mean(0).mean(0)
        return(psds_mean)

def do_resting_state_processing(sr, duration,
                               EEG_CH_NAMES, unused_channels, info,
                               filter_values, reference_ica, band):
    alpha_vals = []
    timeout = time.time() + 60 * duration
    while time.time() <= timeout:
        start = time.time()
        power = process_stream(sr, EEG_CH_NAMES, unused_channels, info,
                              filter_values, reference_ica, band)
        alpha_vals.append(power)
        stop = time.time()
        print(stop - start)
    alpha_mean= np.mean(alpha_vals)
    alpha_std = np.std(alpha_vals)
    return(alpha_mean, alpha_std)

def do_gaming_processing(sr, rs_mean, rs_std,
                         EEG_CH_NAMES, unused_channels, info, filter_values,
                         reference_ica, band, gaming_callback):
    global paradigm

    alpha_vals = []
    while paradigm == 'gaming':
        power = process_stream(sr, EEG_CH_NAMES, unused_channels, info,
                              filter_values, reference_ica, band)
        alpha_vals.append(power)
        score = ((alpha_vals[-1] - (rs_mean - rs_std)) / (2*rs_std) ) * 2 - 1
        if score > 1:
            score = 1
        elif score < -1:
            score = -1

        if gaming_callback is not None:
            gaming_callback(score)
        else:
            print(score)
    return()

def switch_paradigm(value):
    global paradigm
    paradigm = value

def client(headsetname, resting_callback, gaming_callback):
    global paradigm

    # Connect to redis
    amp_name = headsetname

    # Connect to the EEG
    sr = StreamReceiver(window_size=5, buffer_size=100,
                        amp_name=amp_name, eeg_only=True)
    # Wait to have enought data in the buffer
    time.sleep(5)
    sfreq = sr.get_sample_rate()
    watchdog = qc.Timer()
    tm = qc.Timer(autoreset=True)

    # Processing parameters
    sfreq = 300
    filter_values=[50,100]
    resting_state_duration = 1 # in minutes (s)
    band = [8, 12]
    EEG_CH_NAMES = [
       'TRIGGER', 'P3', 'C3', 'F3', 'Fz', 'F4', 'C4', 'P4', 'Cz', 'Pz',
       'Fp1', 'Fp2', 'T3', 'T5', 'O1', 'O2', 'X3', 'X2', 'F7', 'F8', 'X1',
       'A2', 'T6', 'T4'
    ]
    unused_channels = ['TRIGGER', 'X1', 'X2', 'X3', 'A2']
    ch_types = ['eeg'] * len(EEG_CH_NAMES)
    info = mne.create_info(EEG_CH_NAMES, sfreq, ch_types=ch_types)
    reference_ica = None

    # intialisation
    rs_mean = None
    rs_std = None

    #do the stuff
    while True:
        # Wait instruction from redis
        duration = 1
        if paradigm == 'resting_state':
            print('resting_state')
            rs_mean, rs_std = do_resting_state_processing(sr, duration,
                                           EEG_CH_NAMES, unused_channels, info,
                                           filter_values, reference_ica, band)
            print(rs_mean, rs_std)
            if resting_callback is not None:
                resting_callback(True)
        elif paradigm == 'gaming':
            print('gaming')
            if rs_mean is not None and rs_std is not None:
                do_gaming_processing(sr, rs_mean, rs_std,
                                     EEG_CH_NAMES, unused_channels, info,
                                     filter_values, reference_ica, band, gaming_callback)
                rs_mean = None
                rs_std = None
        elif paradigm == 'demo':
            print('demo')
            print('resting_state')
            rs_mean, rs_std = do_resting_state_processing(sr, duration,
                                           EEG_CH_NAMES, unused_channels, info,
                                           filter_values, reference_ica, band)
            print('gaming')
            do_gaming_processing(sr, rs_mean, rs_std,
                                 EEG_CH_NAMES, unused_channels, info,
                                 filter_values, reference_ica, band, gaming_callback)
            rs_mean = None
            rs_std = None

if __name__ == '__main__':
    client(None, None, None)
