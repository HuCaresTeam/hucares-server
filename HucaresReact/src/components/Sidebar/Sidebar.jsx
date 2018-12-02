import React from 'react';
import { Icon, Menu, Segment, Sidebar } from 'semantic-ui-react';
import MapContainer from '../Map/MapContainer';
import 'semantic-ui-css/semantic.min.css';

export class SidebarContainer extends React.Component {
  render() {
    return (
      <Sidebar.Pushable as={Segment}>
        <Sidebar as={Menu} animation="push" icon="labeled" direction="" inverted vertical visible>
          <Menu.Item as="a" href="/">
            <Icon name="home" />
            Home
          </Menu.Item>
          <Menu.Item as="a" href="/mlp">
            <Icon name="play" />
            Missing license plates
          </Menu.Item>
          <Menu.Item as="a" href="/dlp">
            <Icon name="shield alternate" />
            Detected License plates
          </Menu.Item>
          <Menu.Item as="a" href="/cameras">
            <Icon name="camera" />
            Cameras
          </Menu.Item>
        </Sidebar>

        <Sidebar.Pusher>
          <MapContainer />
        </Sidebar.Pusher>
      </Sidebar.Pushable>
    );
  }
}