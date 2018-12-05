import React from 'react';
import { Pagination, Table } from 'semantic-ui-react';
import styles from './CamerasTable.scss';
import cameraMock from '../../mocks/camera';
import { chunkArray } from '../../utils/Array';

export class CamerasTable extends React.Component {
  state = { activePage: 1 };

  getPaginatedData() {
    return chunkArray(cameraMock, 15);
  }

  handlePaginationChange = (e, { activePage }) => this.setState({ activePage });

  render() {
    const { activePage } = this.state;
    const data = this.getPaginatedData();

    return (
      <div className={styles.camerasTable}>
        <Table celled padded>
          <Table.Header>
            <Table.Row>
              <Table.HeaderCell>Camera URL</Table.HeaderCell>
              <Table.HeaderCell>Latitude</Table.HeaderCell>
              <Table.HeaderCell>Longitude</Table.HeaderCell>
              <Table.HeaderCell>Is trusted source</Table.HeaderCell>
            </Table.Row>
          </Table.Header>
          <Table.Body>
            {!!data &&
              !!data[activePage - 1] &&
              data[activePage - 1].map(obj => (
                <Table.Row key={obj.Id}>
                  <Table.Cell>{obj.HostUrl}</Table.Cell>
                  <Table.Cell>{obj.Latitude}</Table.Cell>
                  <Table.Cell>{obj.Longitude}</Table.Cell>
                  <Table.Cell>{obj.IsTrustedSource ? `Trusted` : `Not trusted`}</Table.Cell>
                </Table.Row>
              ))}
          </Table.Body>

          <Table.Footer>
            <Table.Row>
              <Table.HeaderCell colSpan="4">
                <Pagination
                  activePage={activePage}
                  firstItem={null}
                  lastItem={null}
                  pointing
                  secondary
                  totalPages={data.length}
                  onPageChange={this.handlePaginationChange}
                />
              </Table.HeaderCell>
            </Table.Row>
          </Table.Footer>
        </Table>
      </div>
    );
  }
}
