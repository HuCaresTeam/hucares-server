import React from 'react';
import { Table } from 'semantic-ui-react';
import axios from 'axios';
import PaginationContainer from '../../components/Pagination/Pagination';
import styles from './CamerasTable.scss';
import 'semantic-ui-css/semantic.min.css';
import { CameraImageModal } from '../../components/Modal/CameraModal';
import { chunkArray } from '../../utils/Array';
import { CameraDeleteModal } from '../../components/Modal/DataHelpers/CameraHelper/CameraDeleteModal';
import { CameraDataChangeModal } from '../../components/Modal/DataHelpers/CameraHelper/CameraDataChangeModal';
import { InfoEditingModal } from '../../components/Modal/InfoEditingModal';

export class CamerasTable extends React.Component {
  state = {
    activePage: 1,
    data: [],
  };

  componentDidMount() {
    axios
      .get(`${process.env.HUCARES_API_BASE_URL}/api/camera/all`, {
        headers: { 'Access-Control-Allow-Origin': '*' },
      })
      .then(res => {
        const data = chunkArray(res.data, window.innerHeight > 800 ? 10 : 6);
        this.setState({ data });
      })
      .catch(() => {
        this.setState({ data: [] });
      });
  }

  handlePaginationChange = (e, { activePage }) => this.setState({ activePage });

  createModalInfo() {
    return {
      triggerButtonText: 'Add camera',
      triggerButtonStyle: 'ui primary right floated button',
      modalHeaderText: 'New camera',
      formFields: [
        {
          id: 0,
          label: 'Camera Host URL',
          placeHolderText: 'host url',
          value: undefined,
        },
        {
          id: 1,
          label: 'Latitude',
          placeHolderText: 'latitude',
          value: undefined,
        },
        {
          id: 2,
          label: 'Longitude',
          placeHolderText: 'longitude',
          value: undefined,
        },
      ],
      checkboxes: [
        {
          id: 0,
          label: 'This camera is trusted source',
          value: false,
        },
      ],
      submitButtonText: 'Submit',
      cancelButtonText: 'Cancel',
    };
  }

  render() {
    const { activePage } = this.state;
    const cameraData = this.state.data;

    return (
      <div className={styles.camerasTable}>
        <Table celled padded>
          <Table.Header>
            <Table.Row>
              <Table.HeaderCell>Camera URL</Table.HeaderCell>
              <Table.HeaderCell>Latitude</Table.HeaderCell>
              <Table.HeaderCell>Longitude</Table.HeaderCell>
              <Table.HeaderCell>Is trusted source</Table.HeaderCell>
              <Table.HeaderCell>Action</Table.HeaderCell>
            </Table.Row>
          </Table.Header>
          <Table.Body>
            {!!cameraData &&
              !!cameraData[activePage - 1] &&
              cameraData[activePage - 1].map(obj => (
                <Table.Row key={obj.Id}>
                  <Table.Cell>
                    <CameraImageModal imageUrl={obj.HostUrl} />
                  </Table.Cell>
                  <Table.Cell>{obj.Latitude}</Table.Cell>
                  <Table.Cell>{obj.Longitude}</Table.Cell>
                  <Table.Cell>{obj.IsTrustedSource ? `Trusted` : `Not trusted`}</Table.Cell>
                  <Table.Cell>
                    <CameraDataChangeModal data={obj} />
                    <CameraDeleteModal />
                  </Table.Cell>
                </Table.Row>
              ))}
          </Table.Body>

          <Table.Footer>
            <Table.Row>
              <Table.HeaderCell colSpan="5">
                <PaginationContainer
                  activePage={activePage}
                  totalPages={cameraData.length}
                  onPageChange={this.handlePaginationChange}
                />

                <InfoEditingModal data={this.createModalInfo()} />
              </Table.HeaderCell>
            </Table.Row>
          </Table.Footer>
        </Table>
      </div>
    );
  }
}
