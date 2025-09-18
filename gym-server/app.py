import gym_server
import numpy as np
import gymnasium as gym
from gymnasium import spaces

import os

def main():

    # Server
    serverIP = '127.0.0.1'
    serverPort = '8000'

    # Godot game exe command
    projectPath = "../gym-godot" # '/home/user/.../GymGodot/gym-godot/' (project.godot folder)
    godotPath = 'E:\\program\\godot\\Godot_v4.4.1-stable_mono_win64\\Godot_v4.4.1-stable_mono_win64\\Godot_v4.4.1-stable_mono_win64.exe'
    scenePath = './examples/pendulum/Root.tscn'
    exeCmd = 'cd {} & {} {}'.format(projectPath, godotPath, scenePath)

    print(f"Running with argument: \n\t{exeCmd}")

    # Action Space ('go left' (0) or 'go right' (1))
    action_space = spaces.Discrete(2)

    # Observation Space (Cart Position, Cart Velocity, Pole Angle, Pole Angle Velocity)
    observation_space = spaces.Box(low=np.array([-40, -np.inf, -np.pi/8, -np.inf], dtype=np.float32), 
                                high=np.array([40, np.inf, np.pi/8, np.inf], dtype=np.float32),
                                dtype=np.float32)

    # Create folder to store renders
    renderPath = os.getcwd() + '/render_frames/' # '/home/user/.../GymGodot/gym-godot/examples/cartpole/render_frames'
    if not os.path.exists(renderPath):
        os.makedirs(renderPath)

    print("made it 1")

    ####################

    # Set up gym-server with those parameters
    env = gym.make('server-v0', serverIP=serverIP, serverPort=serverPort, exeCmd=exeCmd, 
                action_space=action_space, observation_space=observation_space, 
                window_render=True, renderPath=renderPath)

    print("made it 2")

    print(env.reset())

    print("made it 3")


    for i in range(0,5):
        print(env.step(0)) # "go left"
        # or env.step(1) for "go right"


    print(env.reset())

    print("made it 4")

    env.render()

    import matplotlib.pyplot as plt
    import matplotlib.image as mpimg
    img = mpimg.imread(renderPath + '0.png')
    plt.imshow(img)
    plt.axis('off')
    plt.show()

    env.close()
    del env
    print("made it 5")

if __name__ == '__main__':
    main()