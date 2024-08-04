import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import(`./features/preview/preview.component`)
            .then(x => x.PreviewComponent)
    }
];
