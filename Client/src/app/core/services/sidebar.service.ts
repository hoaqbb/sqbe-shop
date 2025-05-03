import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SidebarService {
  sidebarStates = signal<Map<string, boolean>>(new Map());

  open(id: string) {
    const map = new Map<string, boolean>();
    map.set(id, true);
    this.sidebarStates.set(map);
  }

  close(id: string) {
    const current = new Map(this.sidebarStates());

    if (current.has(id)) {
      current.set(id, false);
      this.sidebarStates.set(current);
    }
  }

  closeAll() {
    const current = new Map(this.sidebarStates());
    for (const key of current.keys()) {
      current.set(key, false);
    }
    this.sidebarStates.set(current);
  }

  toggle(id: string) {
    const current = new Map(this.sidebarStates());
    const isOpen = current.get(id) ?? false;

    const newMap = new Map<string, boolean>();
    newMap.set(id, !isOpen);

    this.sidebarStates.set(newMap);
  }

  isOpen(id: string): boolean {
    return this.sidebarStates().get(id) ?? false;
  }
}
