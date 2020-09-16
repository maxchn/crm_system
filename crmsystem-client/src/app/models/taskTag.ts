import { Tag } from './tagModel';
import { Task } from './taskModel';

export class TaskTag {
    taskId: number;
    task: Task;
    tagId: number;
    tag: Tag;
}