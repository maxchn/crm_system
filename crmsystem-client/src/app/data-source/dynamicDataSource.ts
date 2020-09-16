import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, merge } from 'rxjs';
import { FlatTreeControl } from '@angular/cdk/tree';
import { DynamicFlatNode } from '../models/dynamicFlatNode';
import { CollectionViewer, SelectionChange } from '@angular/cdk/collections';
import { map } from 'rxjs/operators';
import { HttpService } from '../services/http.service';
import { DataService } from '../services/data.service';
import { Utils } from '../core/utils';

@Injectable()
export class DynamicDataSource {

    dataChange = new BehaviorSubject<DynamicFlatNode[]>([]);
    fileType: number = 3;

    get data(): DynamicFlatNode[] { return this.dataChange.value; }
    set data(value: DynamicFlatNode[]) {
        this._treeControl.dataNodes = value;
        this.dataChange.next(value);
    }

    constructor(private _treeControl: FlatTreeControl<DynamicFlatNode>,
        private _httpService: HttpService,
        private _dataService: DataService) { }

    connect(collectionViewer: CollectionViewer): Observable<DynamicFlatNode[]> {
        this._treeControl.expansionModel.onChange.subscribe(change => {
            if ((change as SelectionChange<DynamicFlatNode>).added ||
                (change as SelectionChange<DynamicFlatNode>).removed) {
                this.handleTreeControl(change as SelectionChange<DynamicFlatNode>);
            }
        });

        return merge(collectionViewer.viewChange, this.dataChange).pipe(map(() => this.data));
    }

    /** Handle expand/collapse behaviors */
    handleTreeControl(change: SelectionChange<DynamicFlatNode>) {
        if (change.added) {
            change.added.forEach(node => this.toggleNode(node, true));
        }
        if (change.removed) {
            change.removed.slice().reverse().forEach(node => this.toggleNode(node, false));
        }
    }

    /**
     * Toggle the node, remove from display list
     */
    async toggleNode(node: DynamicFlatNode, expand: boolean) {

        if (expand) {
            const children = await this._httpService.getFilesListPromise(node.path, this._dataService.getCompanyId(), this.fileType);
            const index = this.data.indexOf(node);

            // If no children, or cannot find the node, no op
            if (!children || index < 0) {
                node.expandable = false;
                node.isLoading = true;
                this.dataChange.next(this.data);
                return;
            }

            node.isLoading = true;

            let nodes = [];
            for (let i = 0; i < children.length; i++) {
                Utils.printValueToConsole(node.level);
                nodes.push(new DynamicFlatNode(children[i].name, node.level + 1, children[i].type === 1,
                    false, children[i].path));
            }

            this.data.splice(index + 1, 0, ...nodes);
        }
        else {
            const index = this.data.indexOf(node);

            let count = 0;
            for (let i = index + 1; i < this.data.length
                && this.data[i].level > node.level; i++ , count++) { }
            this.data.splice(index + 1, count);
        }

        // notify the change
        this.dataChange.next(this.data);
        node.isLoading = false;
    }
}