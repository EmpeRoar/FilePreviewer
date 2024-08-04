import { Component } from '@angular/core';
import { NgxDocViewerModule } from 'ngx-doc-viewer';

@Component({
  selector: 'app-preview',
  standalone: true,
  imports: [
    NgxDocViewerModule
  ],
  templateUrl: './preview.component.html',
  styleUrl: './preview.component.scss'
})
export class PreviewComponent {
  doc: string = `http://localhost:3200/word.docx`;
  constructor() {

  }
}
